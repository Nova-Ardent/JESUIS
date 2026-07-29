# Known Issues — Screen Persistence

Found during review of `feature/screen-persistence` against `main`. Everything here was
deliberately left unfixed on that branch: each one either changes behaviour rather than
repairing it, or could not be verified without running the editor.

Bugs found in the same review that *were* fixed (null root on deserialize, `SaveAs` over
an existing file, `SaveAs` onto the current file, dropped instance in
`PromptToSaveChanges`) are not listed — see the branch history.

| # | Issue | Severity | Why it is still open |
|---|-------|----------|----------------------|
| 1 | Discard does not discard | Data loss (silent) | Fix changes behaviour, not a repair |
| 2 | `Open` accepts paths outside the project | Cosmetic | Cryptic error, no bad state |
| 3 | `Ctrl+S` overlaps Unity's global Save | Unknown | Cannot verify without the editor |
| 4 | `plans.meta` will be untracked | Housekeeping | Needs Unity to generate the file |

---

## 1. "Discard" leaves the edits in place

*Location:* [MainWindow.cs:73](Editor/UIBuilder/MainWindow.cs#L73),
[MainWindow.cs:81](Editor/UIBuilder/MainWindow.cs#L81)

Closing the window with unsaved changes prompts Save / Discard. Choosing **Discard** does
nothing beyond skipping the write — and that is not enough.

Edits mutate the `Screen` `ScriptableObject` in memory. That instance is the one the
`AssetDatabase` is caching for the path it was loaded from. Nothing evicts it, so the next
`AssetDatabase.LoadAssetAtPath<Screen>` in `TryRestoreLastOpened` hands back **the same
mutated instance**, not a fresh read of the file. The discarded edits reappear.

The file on disk is never wrong — `EditorUtility.SetDirty` is only ever called at save
time, so Unity has no reason to flush the object itself. The staleness is purely in the
cached instance.

**Reproduce:** save a screen, move an element, close the window, choose Discard, reopen
the window. The element is where you left it, not where the `.asset` says it is.

**Fix sketch:** on discard, force the cached instance back in line with the file:

```csharp
string path = AssetDatabase.GetAssetPath(screen);
if (!string.IsNullOrEmpty(path))
{
    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
}
```

A reimport re-reads the (unchanged) file into the existing instance. It is not free, and it
makes Discard cost a round trip through the importer, which is why this is a judgement call
rather than an obvious repair. A screen that was never saved has no path and needs nothing —
the instance is simply abandoned.

---

## 2. `Open` accepts a file from outside the project

*Location:* [ScreenAssetManager.cs:119](Editor/UIBuilder/Data/ScreenAssetManager.cs#L119)

`EditorUtility.OpenFilePanel` starts in `Application.dataPath` but does not confine the user
there. Pick anything outside the project and
[`GetProjectRelativePath`](Editor/Utilities/System/PathUtils/GetProjectRelativePath.cs#L13)
dutifully produces `../../foo.asset`, `AssetDatabase.LoadAssetAtPath` returns null, and the
user gets:

```
Failed to open screen at: ../../foo.asset
```

No bad state results — the open is abandoned and the current screen is untouched — but the
message explains nothing. An `Assets/` prefix check before `Open(string)` would let the
error say what actually went wrong.

---

## 3. `Ctrl+S` overlaps Unity's built-in Save — unverified

*Location:* [MainWindow.cs:202](Editor/UIBuilder/MainWindow.cs#L202)

```csharp
[Shortcut("JESUIS/Save Screen", typeof(MainWindow), KeyCode.S, ShortcutModifiers.Action)]
```

This binds the same chord as Unity's global File/Save. Window-scoped shortcuts are supposed
to take priority over global ones while the window has focus, so this should be fine and
should not raise a conflict in the Shortcut Manager — but that is reasoning about
documented behaviour, not an observation. It is the one item in this review that needs the
editor to settle.

**Check:** open the tool, focus it, press `Ctrl+S`, and confirm the screen saves and the
project does not. Then check **Edit → Shortcuts** for a conflict marker on `JESUIS/Save
Screen`.

---

## 4. `plans.meta` is not covered by `.gitignore`

*Location:* [.gitignore:71](.gitignore#L71)

The repo lives inside a Unity project's `Assets/`, so Unity generates a `.meta` for every
file and folder it sees, including this `plans/` directory and the markdown in it.

`.gitignore` line 71 ignores `SCREEN_PERSISTENCE.md.meta`. That pattern has no slash, so it
still matches after the move into `plans/` — the file metas stay ignored. The **folder**
meta, `plans.meta`, is not covered by any rule and will show up untracked the first time
Unity scans the project.

CLAUDE.md's rule is that `.meta` files are committed and stay in sync, so `plans.meta`
should be committed once Unity has generated it. `KNOWN_ISSUES.md.meta` needs an ignore
entry alongside its sibling, or the two markdown metas should be committed instead — either
is consistent, the current mix is not.
