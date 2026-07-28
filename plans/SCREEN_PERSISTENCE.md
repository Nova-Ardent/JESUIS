# Screen Persistence — Implementation Plan

Save and load the authored `Screen` as a Unity `.asset`, so work survives closing the
window and — more importantly — surviving domain reload on script recompile.

---

## 1. Current state

`MainWindow.CreateGUI` calls `CreateInstance<Screen>()` on every open
([MainWindow.cs:29](Editor/UIBuilder/MainWindow.cs#L29)). There is no
`AssetDatabase.CreateAsset`, `SaveAssets`, or `SetDirty` anywhere in the repo. The
element tree is discarded when the window closes and when Unity recompiles.

Four things block a naive "just call `CreateAsset`":

| # | Blocker | Location |
|---|---------|----------|
| 1 | `Screen.rootElement` is private with no serialization attribute — the asset would save empty | [Screen.cs:9](Shared/ScreenData/Screen.cs#L9) |
| 2 | `BaseElement.children` is private with no accessor — nothing can walk a loaded tree | [BaseElement.cs:13](Shared/ScreenData/Data/BaseElement.cs#L13) |
| 3 | No code path builds views from an *existing* tree. Hierarchy items and renderer elements are only ever created one-at-a-time by the `ChildAdded` broadcast | [Hierarchy.cs:27](Editor/Elements/Widgets/Hierarchy.cs#L27), [RendererHierarchyController.cs:49](Editor/UIBuilder/Panels/Views/Renderer/Hierarchy/RendererHierarchyController.cs#L49) |
| 4 | Views capture the root at construction time, so `CurrentScreen` cannot be swapped | [HierarchyView.cs:25](Editor/UIBuilder/Panels/Views/HierarchyView.cs#L25), [RendererView.cs:31](Editor/UIBuilder/Panels/Views/RendererView.cs#L31) |

Blocker 3 is the bulk of the work. Save is nearly free; **load is the feature**.

---

## 2. Design decisions

**Asset format — `ScriptableObject` + `[SerializeReference]`.** `Screen` is already a
`ScriptableObject` and `BaseElement.children` already carries `[SerializeReference]`, so
the data model was built for this. No custom JSON layer.

**Save is explicit, not autosaved.** `UIEditorLayoutManager` flushes on
`OnInspectorUpdate` (~10×/sec), which is fine for `EditorPrefs` and wrong for
`AssetDatabase`. Screen writes happen on an explicit Save / Save As.

**Dirty tracking reuses the existing broadcast.** `EditorState.ListenToElementIsDirty`
already fires on every mutation from every view. The asset manager subscribes once and
flips a flag — no new plumbing.

**`CurrentScreen` becomes a `ReactiveProperty<Screen>`**, matching `SelectedElement`.
Views rebuild in a `ListenTo` handler instead of reading the field in their constructors.

---

## 3. Phases

Each phase is independently verifiable. Phases 0–2 deliver a working roundtrip; 3–4 make
it pleasant.

### Phase 0 — Make the data model actually serialize

*Files:* `Shared/ScreenData/Screen.cs`, `Shared/ScreenData/Data/BaseElement.cs`

```csharp
// Screen.cs
[SerializeReference] RootElement rootElement = new RootElement();
```

```csharp
// BaseElement.cs — required by every recursive walk in Phase 2
public IReadOnlyList<BaseElement> GetChildren() => children;
```

**Verify:** temporary menu item that calls `AssetDatabase.CreateAsset` on a tree with two
nested children, then open the `.asset` in a text editor. `[SerializeReference]` writes a
distinctive `references: version: 2 / RefIds:` block — each element should appear there
with its `type: {class: EmptyElement, ns: JESUIS.Shared.ScreenData.Data, ...}`. If that
block is missing or empty, stop here; nothing downstream will work.

---

### Phase 1 — `ScreenAssetManager`

*New file:* `Editor/UIBuilder/Data/ScreenAssetManager.cs`

Mirrors `UIEditorLayoutManager` in role and placement.

```csharp
public class ScreenAssetManager
{
    const string LAST_SCREEN_PATH_KEY = "JESUIS_CurrentScreenPath";

    bool isDirty = false;
    string currentPath = null;

    public bool IsDirty => isDirty;
    public string CurrentPath => currentPath;

    public ScreenAssetManager(EditorState editorState)
    {
        editorState.ListenToElementIsDirty((view, change) => isDirty = true);
    }

    public Screen New();                       // CreateInstance, currentPath = null
    public bool Save(Screen screen);           // SaveAs if currentPath == null
    public bool SaveAs(Screen screen);         // EditorUtility.SaveFilePanelInProject
    public Screen Open();                      // EditorUtility.OpenFilePanel -> relative path
    public Screen Open(string path);           // AssetDatabase.LoadAssetAtPath<Screen>
    public Screen TryRestoreLastOpened();      // EditorPrefs -> Open(path), null on miss
}
```

`Save` on a known path is `EditorUtility.SetDirty(screen)` + `AssetDatabase.SaveAssets()`;
on an unknown path it is `AssetDatabase.CreateAsset(screen, path)` + `SaveAssets`. Both
clear `isDirty` and write `currentPath` to `EditorPrefs`.

Note `EditorUtility.OpenFilePanel` returns an **absolute** path; it must be converted
relative to the project root before `LoadAssetAtPath`. `Utilities.GetUSS` already does
this dance with `Path.GetRelativePath(Path.GetDirectoryName(Application.dataPath), …)` —
worth extracting that into `PathUtils` rather than duplicating it a third time
(`ResourceLoader` has its own copy).

**Verify:** save a tree, reopen Unity, `Open(path)` it, and log
`GetRootElement().GetChildren().Count`. Views will still be empty — that is Phase 2.

---

### Phase 2 — Rebuild views from an existing tree

The core of the feature.

#### 2a. Prerequisite: fix renderer type resolution

`RendererElementLoader.InstantiateRendererElement<T>` resolves the renderer from
`typeof(T)` — the *static* type ([RendererElementLoader.cs:29](Editor/UIBuilder/Panels/Views/Renderer/Hierarchy/Builder/RendererElementLoader.cs#L29)).
Today it is always called with `ChildAdded.Data`, statically typed `BaseElement`, so it
always resolves to `BaseRendererElement` and happens to work. A recursive walk over
`GetChildren()` has the same static type, so **every loaded element would render as the
base type** the moment a second element type exists.

Switching to `data.GetType()` alone breaks it differently: `EmptyElement` has no
registered renderer, so `GetRendererElementType` returns `null` and `Activator.CreateInstance`
throws. Three coupled changes:

1. `GetRendererElementType` walks up `BaseType` until it finds a registration, so
   `EmptyElement` falls back to `BaseRendererElement` naturally.
2. Add a non-generic `SetData(BaseElement)` to `IRendererElement` — the current
   `resultValue is IRendererElement<T>` pattern-match cannot succeed once `T` is a
   runtime type that differs from the interface's generic argument.
3. Add a non-generic `InstantiateRendererElement(BaseElement data)` overload using
   `data.GetType()`; keep the generic one delegating to it.

#### 2b. Recursive builders

*Files:* `HierarchyView.cs`, `RendererHierarchyController.cs`

Both walk the same tree; neither currently has a recursive path.

```csharp
// HierarchyView — pair each BaseElement with a HierarchyItem
void BuildSubtree(BaseElement data, HierarchyItem parentItem)
{
    foreach (BaseElement child in data.GetChildren())
    {
        HierarchyItem item = new HierarchyItem(child, GetActions, OnElementClicked);
        parentItem.AddChild(item);
        BuildSubtree(child, item);
    }
}
// ...then a single editorHierarchy.RebuildListVisuals() at the end.
```

```csharp
// RendererHierarchyController — clear map, re-seed root, rebuild
public void SetScreen(Screen screen)
{
    Clear();
    elementToRendererElementMap.Clear();
    elementToRendererElementMap.Add(screen.GetRootElement(), this);
    BuildSubtree(screen.GetRootElement(), this);
}
```

`BuildSubtree` here must repeat what `OnElementIsDirty`'s `ChildAdded` branch already
does per-element — instantiate, `Add`, call `OnValuesChanged()`, register the parent's
`GeometryChangedEvent` callback, record in the map. Factor that into one
`AttachRendererElement(BaseElement, VisualElement parent)` used by both paths rather than
letting the two drift.

**Order matters:** `OnValuesChanged` resolves `Percentage` units against
`parent.contentRect`, which is zero until layout runs. Build the tree, then let the
existing `GeometryChangedEvent` → `OnParentGeometryChanged` chain correct the sizes on
the next layout pass. Do not fight it with `resolvedStyle` reads during construction.

**Verify:** save a nested tree, reopen the window, confirm it appears in both the
hierarchy and the renderer, with correct positions for both Pixels and Percentage units.

---

### Phase 3 — Swappable current screen

*Files:* `EditorState.cs`, `HierarchyView.cs`, `RendererView.cs`, `RendererDisplay.cs`, `MainWindow.cs`

```csharp
// EditorState
public ReactiveProperty<Screen> CurrentScreen = new ReactiveProperty<Screen>(null);
```

Each view subscribes with `ListenTo` and calls its Phase 2 builder. Load sequence:

```csharp
editorState.SelectedElement.Value = null;   // must come first
editorState.CurrentScreen.Value = loadedScreen;
```

The null-first ordering is not optional. `RendererHierarchyController.OnSelectedElementChanged`
indexes `elementToRendererElementMap[selectedElement]` with a raw indexer
([line 42](Editor/UIBuilder/Panels/Views/Renderer/Hierarchy/RendererHierarchyController.cs#L42));
a selection left pointing into the old tree throws `KeyNotFoundException` after the map is
cleared. The `null` path is already handled correctly (`SetActive(false)`).

Also clear `HierarchyView.hierarchyItem` during rebuild — it caches the last-clicked item
and `OnElementIsDirty` calls `UpdateLabel()` on it unguarded.

`RendererDisplay` should keep its zoom/pan and `RenderTexture` across a screen swap; only
the `RendererHierarchyController` contents need rebuilding.

**Verify:** with screen A open and an element selected, open screen B. No exceptions, both
views swap, box selector clears.

---

### Phase 4 — Entry points and unsaved-change handling

*Files:* `MainWindow.cs`

`MainWindow.GetContextMenuOptions()` already exists and already yields the split actions —
add `New` / `Open` / `Save` / `Save As` there. For a Ctrl+S shortcut, add a static
`[MenuItem("JESUIS/Screen/Save %s")]` that resolves the open window via
`EditorWindow.HasOpenInstances<MainWindow>()` + `GetWindow`.

- Title bar shows `JESUIS — <screen name>*` while `IsDirty`.
- `OnDestroy` prompts via `EditorUtility.DisplayDialogComplex` when dirty.
- `CreateGUI` calls `TryRestoreLastOpened()` and falls back to `New()`.

**Verify:** dirty marker appears on first edit and clears on save; closing dirty prompts;
reopening restores the last screen.

---

## 4. Risks and known limitations

**`[SerializeReference]` is fragile across type moves.** Managed references store the
class name and namespace. Renaming or moving an element type silently orphans it in
existing assets. This repo has already done one such rename (`568c13a`, *"Name space
change from ScreenData.ScreenDataTypes, to ScreenData.Data"*). Once assets exist in the
wild, element types need
`[UnityEngine.Scripting.APIUpdating.MovedFrom]` when they move.

**Domain reload still loses unsaved edits.** An asset on disk fixes window-close, but a
script recompile with unsaved changes still discards them — the window is rebuilt and the
in-memory tree is gone. A full fix means `[SerializeField]` state on the `EditorWindow` or
a `ScriptableSingleton` scratch buffer. Out of scope here; worth a follow-up once save
exists, because Phase 4's dirty flag is the hook it would use.

**No undo.** Unblocked by this work (`Undo.RecordObject` needs a `UnityEngine.Object`, and
`Screen` is one) but explicitly not included. Note that `Undo` on `[SerializeReference]`
graphs records the whole object — acceptable at this tree size, worth measuring later.

---

## 5. Sequencing summary

| Phase | Deliverable | Blocking? |
|-------|-------------|-----------|
| 0 | `Screen`/`BaseElement` serialize and expose children | Yes — everything |
| 1 | `ScreenAssetManager`, save/load roundtrip provable by log | Yes |
| 2a | Runtime-type renderer resolution + base-type fallback | Yes — 2b is wrong without it |
| 2b | Recursive hierarchy + renderer reconstruction | Yes — this *is* load |
| 3 | `ReactiveProperty<Screen>`, view rebuild, selection reset | No — single-screen works without it |
| 4 | Menu entries, dirty marker, close prompt, restore last | No — polish |

Phases 0–2 are the minimum that makes the tool's output durable. 3–4 can follow.
