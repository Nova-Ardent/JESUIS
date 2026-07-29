# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

JESUIS is a Unity **Editor** extension: a custom screen/UI authoring tool (`JESUIS/UI Builder` menu item) with its own dockable panel system, hierarchy, inspector, and WYSIWYG renderer. Everything is built from code-driven `VisualElement`s — there are no `.uxml` files and no `[UxmlElement]` registration; USS is used only for static styling.

The repo is a folder that lives **inside a Unity project's `Assets/`**. Asset paths are resolved at runtime relative to `Application.dataPath` (see `ResourceLoader` and `Utilities.GetUSS`), so the tool works from any subfolder of `Assets/`, but only from inside `Assets/`.

## Build / test / verify

There is no build script, test suite, linter, or CI. Unity itself is the compiler:

- Open the containing Unity project; Unity recompiles on file save and reports errors in the Console.
- Launch the tool via the **JESUIS → UI Builder** menu item (`MainWindow.ShowWindow`).
- To reset a corrupt saved layout, delete the `JESUIS_UIEditorLayout` EditorPrefs key (the loader also deletes it on deserialization failure).

`.meta` files are committed and must stay in sync — new files need Unity to generate their `.meta` before committing.

## Assemblies

Two asmdefs, and the split matters:

- `Shared/JeSuisShared.asmdef` — runtime-safe, no editor references. Holds the serializable data model only.
- `Editor/JeSuisEditor.asmdef` — `includePlatforms: ["Editor"]`, references `JeSuisShared`.

Anything referencing `UnityEditor` must stay under `Editor/`. The data model (`Screen`, `BaseElement`, `Transform`) is deliberately editor-free so it can eventually be consumed at runtime.

## Data model (`Shared/ScreenData/`)

`Screen` (a `ScriptableObject`) owns a single `[SerializeReference] RootElement`. `BaseElement` is the node type: `Name`, a `Transform`, and a `[SerializeReference] List<BaseElement> children` exposed read-only through `GetChildren()` (with `EnumerateSubtree()` for the depth-first walk every recursive consumer shares) — polymorphic children rely on `SerializeReference`, so new element types must be `[System.Serializable]` classes deriving from `BaseElement` (not `ScriptableObject`s). Both `SerializeReference` fields are what make the `.asset` roundtrip work, so neither may be downgraded to `SerializeField`.

`Types.Transform` is *not* `UnityEngine.Transform` — it is 2D and layout-oriented: `Size`, `Position`, `Scale`, `Rotation`, plus `Anchor`/`Pivot` (`Alignment`) and four independent `Unit` (Pixels/Percentage) selectors for horizontal/vertical position and size. Because both `Transform` names are in scope in most editor files, this type is almost always written fully qualified as `Shared.ScreenData.Types.Transform`.

## Editor architecture

### State and the dirty broadcast

`EditorState` is the single hub shared by every view:

- `ReactiveProperty<BaseElement> SelectedElement` — views subscribe via `ListenTo`.
- `ReactiveProperty<Screen> CurrentScreen` — views build themselves in a `ListenTo` handler, *not* in their constructor, so the screen can be swapped at runtime.
- `TriggerElementIsDirty(EditorViews triggeringView, ElementChanges change)` — a broadcast of `ValuesUpdated` / `ChildAdded` / `ChildRemoved` (`Data/StateChanges/ElementChanges.cs`).

**Convention:** the sender passes itself as `triggeringView`, and every handler early-returns when the change came from itself. Forgetting this creates infinite update loops between the inspector, hierarchy, and renderer. Add a new change kind by adding to `ElementChangeType` and subclassing `ElementChanges`/`ElementChanges<T>`.

There is no undo integration — mutations write straight into the data objects, and `EditorUtility.SetDirty` is only applied at save time.

### Screen persistence

`ScreenAssetManager` (`Editor/UIBuilder/Data/`) owns the `.asset` on disk. It subscribes to the dirty broadcast to track unsaved changes, and writes only on an explicit New / Open / Save / Save As — never on a timer, unlike `UIEditorLayoutManager`, because these calls touch the `AssetDatabase`. The last path is remembered in the `JESUIS_CurrentScreenPath` EditorPrefs key and restored on the next `CreateGUI`.

`EditorState.SetCurrentScreen` is the only place a screen is swapped, and the order inside it is load-bearing: **`SelectedElement` must be cleared before `CurrentScreen` is assigned**, otherwise the views rebuild their element→visual maps while a selection still points into the outgoing tree. Assign through that method rather than writing `CurrentScreen.Value` directly. `MainWindow.SetCurrentScreen` wraps it with the null / same-instance guard (saving hands back the instance it was given) and the title refresh.

Dirty state lives on `ScreenAssetManager.IsDirty`, a `ReactiveProperty<bool>` the window subscribes to for the title's `*` marker. `OnElementIsDirty` keeps an explicit early-return because it runs for every drag tick and keystroke.

Because `[SerializeReference]` stores class and namespace names, moving or renaming an element type orphans it in existing assets — such types need `[UnityEngine.Scripting.APIUpdating.MovedFrom]`. Note that a script recompile with unsaved changes still discards them; the asset only survives closing the window.

### Panels and layout (manual, not flexbox)

Sizing is propagated by hand, not by the UIElements layout engine. `BaseWindow` listens for `GeometryChangedEvent` and calls `Resize(width, height)` on children implementing `IResizable`; `SplittablePanel` recursively divides that space and re-calls `Resize` on its two children. Most elements set `Position.Absolute` and explicit `style.left/top/width/height`.

`SplittablePanel` is a binary tree — each node holds either one element or two plus a `DragBar`. `Collapse` calls `RepairEmptyChains` to flatten the redundant split-inside-split nodes the split flow creates. `UIEditorLayoutManager` serializes that tree to the `JESUIS_UIEditorLayout` EditorPrefs key; saving is deferred — call `layoutManager.QueueEditorPreferenceUpdate()` after any layout mutation, and `MainWindow.OnInspectorUpdate` flushes it.

### Views

`EditorViews` is the base `VisualElement` for a view, tagged by the `EditorViews.Views` enum. `ViewManager` holds **one instance of each view** for the whole window, so two panels showing the same view share the object (hence the `RegisterOnViewChanged` reset that flips a duplicate panel back to `None`).

To add a view: add an enum member, subclass `EditorViews` overriding `Type`, construct it in `ViewManager`, and return it from `GetView`. The panel dropdown is built from the enum in declaration order and indexed by `(int)view`, so enum order is load-bearing for saved layouts.

Views optionally contribute tab-bar widgets via `GetActiveTabOptions()` (e.g. `RendererView` returns `AspectRatioDropDown`).

### Renderer element registry

`RendererElementLoader` (lazy singleton) reflects over all loaded assemblies at first use and maps data type → renderer type. To render a new element type, the renderer class must:

1. Carry `[RendererElement(typeof(YourElement))]`,
2. Implement `IRendererElement<YourElement>` (matching the attribute's type), and
3. Derive from `VisualElement` — in practice from `BaseRendererElement`.

Violations are reported as `Debug.LogError` at scan time, not compile time.

`InstantiateRendererElement(BaseElement data)` resolves the renderer from `data.GetType()` — the *runtime* type, so a tree walked back off disk renders as the right types — and `GetRendererElementType` walks up `BaseType` until it finds a registration, which is why `EmptyElement` falls back to `BaseRendererElement`. The non-generic `IRendererElement.SetData` exists for the same reason: `resultValue is IRendererElement<T>` cannot pattern-match when `T` is only known at runtime.

`BaseRendererElement.OnValuesChanged()` is where `Transform` becomes real style: it resolves Pixels/Percentage against the parent's `contentRect`, computes `anchorOffset - pivotOffset` for `left`/`top`, and sets `transformOrigin`/`rotate`/`scale`. `RendererHierarchyController` keeps the `BaseElement → VisualElement` map and is itself the renderer element for the root.

`RendererHierarchyController` is also the one place renderer elements are attached and detached: `AttachRendererElement` / `DetachRendererElement` are shared by the incremental `ChildAdded`/`ChildRemoved` path and by the recursive `SetScreen` rebuild, so the two cannot drift. Detaching must unregister the parent's `GeometryChangedEvent` callback *before* removing the element — a stale callback fires `OnValuesChanged` on a parentless element and throws.

`BoxSelector` + `DragPoint` are the inverse path: drag deltas are converted back through `GetRelativeDelta` (custom `VisualElement` extension) and divided by parent size when the corresponding `Unit` is `Percentage`, then written into the `Transform` and rebroadcast as `ValuesUpdated`.

### Inspector

`InspectorView` is reflection-driven: it walks the selected element's fields (public or `[SerializeField]`, including base types, `DistinctBy` name) and dispatches on field type in `GetInspectorElement`. Adding support for a new field type means adding a case there. Simple types map to elements in `Elements/Input/`; compound types get a dedicated element in `Elements/CompoundInputs/` exposing a static `RegisterField(...)` (see `TransformInputElement`).

Each registered field also appends to `onSelectedElementUpdated`, which is how external edits (dragging in the renderer) push values back into the fields.

## Conventions used throughout

- **Manual event accumulation.** Instead of C# `event`, the codebase stores a plain `Action` and does `if (x == null) x = handler; else x += handler;`. `RegisterOnValueChanged`, `ListenTo`, `ListenToElementIsDirty`, `RegisterOnViewChanged` all follow this. Match it rather than introducing `event`.
- **`SetValueWithoutNotify` vs `SetValue`.** Widgets expose both; use the former when syncing from state to avoid re-triggering the dirty broadcast.
- **USS loading.** Each `.uss` gets a sibling `XxxUSS.cs` subclassing `USSInstanceLoader<XxxUSS>` and passing the file name to the base constructor. `[CallerFilePath]` resolves the path, so the `.cs` must sit next to its `.uss`. Apply with `element.AddStyle(XxxUSS.StyleSheetInstance, "class-name")`.
- **Resources.** Never hardcode asset paths; add the asset to the nested class tree in `ResourceLoader` and access via `ResourceLoader.Instance.Icons.….Value` / `.Shaders.….Value` (lazily `AssetDatabase.LoadAssetAtPath`ed).
- **Colors.** All colors live in `Editor/Settings/Colors.cs` as `SCREAMING_CASE` constants — no literal `new Color(...)` in element code.
- **Context menus.** Override `GetContextMenuOptions()` (yielding `NamedAction`s) on a `BasePanel`/`BaseWindow` subclass; `ContextMenuBuilder` renders it. Right-click is handled by `BasePanel`'s `PointerDownEvent` with `StopImmediatePropagation`, so the innermost panel wins.
- **Namespaces mirror folders** (`JESUIS.Editor.…`, `JESUIS.Shared.…`), with two deliberate exceptions under `Editor/Utilities/` that extend BCL/Unity namespaces directly (`System.Linq.DistinctBy`, `UnityEngine.UIElements.VisualElementExtensions`) so the extensions are available without extra `using`s.
- **Shaders** are driven through `MaterialRTTVisualElement`, which blits a material into a `RenderTexture` and draws it with `DrawMesh` — the only way to get shader output into a UIElements editor panel here.
