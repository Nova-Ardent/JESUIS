using JESUIS.Editor.Helpers.Utils;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Panels.Views;
using JESUIS.Editor.Utilities.System.PathUtils;
using UnityEditor;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Data
{
    /// <summary>
    /// Owns the on disk representation of the screen being authored. Unlike
    /// <see cref="Panels.UIEditorLayoutManager"/>, which flushes on a timer, writes here only ever
    /// happen through an explicit New / Open / Save / Save As, because they touch the AssetDatabase.
    /// </summary>
    public class ScreenAssetManager
    {
        const string LAST_SCREEN_PATH_KEY = "JESUIS_CurrentScreenPath";
        const string SCREEN_EXTENSION = "asset";
        const string DEFAULT_SCREEN_NAME = "New Screen";

        public ReactiveProperty<bool> IsDirty = new ReactiveProperty<bool>(false);

        public ScreenAssetManager(EditorState editorState)
        {
            editorState.ListenToElementIsDirty(OnElementIsDirty);
        }

        public Shared.ScreenData.Screen CreateNew()
        {
            Shared.ScreenData.Screen screen = ScriptableObject.CreateInstance<Shared.ScreenData.Screen>();
            screen.name = DEFAULT_SCREEN_NAME;

            SetSyncedPath(null);
            return screen;
        }

        /// <summary>
        /// Writes <paramref name="screen"/> back to the path it was opened from, falling back to
        /// <see cref="SaveAs"/> when it has never been written. Returns the screen that is now
        /// being edited, or null when the user cancelled.
        /// </summary>
        public Shared.ScreenData.Screen Save(Shared.ScreenData.Screen screen)
        {
            if (screen == null)
            {
                return null;
            }

            string path = AssetDatabase.GetAssetPath(screen);
            if (string.IsNullOrEmpty(path))
            {
                return SaveAs(screen);
            }

            EditorUtility.SetDirty(screen);
            AssetDatabase.SaveAssetIfDirty(screen);

            SetSyncedPath(path);
            return screen;
        }

        public Shared.ScreenData.Screen SaveAs(Shared.ScreenData.Screen screen)
        {
            if (screen == null)
            {
                return null;
            }

            string path = EditorUtility.SaveFilePanelInProject
                ( "Save Screen"
                , string.IsNullOrEmpty(screen.name) ? DEFAULT_SCREEN_NAME : screen.name
                , SCREEN_EXTENSION
                , "Choose where to save this screen."
                );

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            // CreateAsset refuses objects that already live in a file, so an already saved screen is
            // branched on disk instead. That hands back a different instance to edit from here on.
            string sourcePath = AssetDatabase.GetAssetPath(screen);
            if (!string.IsNullOrEmpty(sourcePath))
            {
                // CopyAsset reads the file, so pending edits have to reach disk before it runs.
                EditorUtility.SetDirty(screen);
                AssetDatabase.SaveAssetIfDirty(screen);

                if (!AssetDatabase.CopyAsset(sourcePath, path))
                {
                    Debug.LogError($"Failed to copy screen from {sourcePath} to {path}");
                    return null;
                }

                return Open(path);
            }

            // CreateAsset writes the file itself, so no further flush is needed here.
            AssetDatabase.CreateAsset(screen, path);

            SetSyncedPath(path);
            return screen;
        }

        public Shared.ScreenData.Screen Open()
        {
            string absolutePath = EditorUtility.OpenFilePanel("Open Screen", Application.dataPath, SCREEN_EXTENSION);
            if (string.IsNullOrEmpty(absolutePath))
            {
                return null;
            }

            return Open(PathUtils.GetProjectRelativePath(absolutePath));
        }

        public Shared.ScreenData.Screen Open(string projectRelativePath)
        {
            Shared.ScreenData.Screen screen = Load(projectRelativePath);
            if (screen == null)
            {
                Debug.LogError($"Failed to open screen at: {projectRelativePath}");
                return null;
            }

            SetSyncedPath(projectRelativePath);
            return screen;
        }

        /// <summary>
        /// Reopens whichever screen was last saved or opened. Returns null when there is nothing to
        /// restore, which is the normal case on a first run.
        /// </summary>
        public Shared.ScreenData.Screen TryRestoreLastOpened()
        {
            // A missing key reads back as an empty path, which Load already rejects.
            string path = EditorPrefs.GetString(LAST_SCREEN_PATH_KEY);
            Shared.ScreenData.Screen screen = Load(path);

            if (screen == null)
            {
                EditorPrefs.DeleteKey(LAST_SCREEN_PATH_KEY);
                return null;
            }

            SetSyncedPath(path);
            return screen;
        }

        Shared.ScreenData.Screen Load(string projectRelativePath)
        {
            if (string.IsNullOrEmpty(projectRelativePath))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<Shared.ScreenData.Screen>(projectRelativePath);
        }

        /// <summary>
        /// Records that the editor is now in sync with the screen at <paramref name="path"/>, which
        /// is empty for a screen that has never been written.
        /// </summary>
        void SetSyncedPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                EditorPrefs.DeleteKey(LAST_SCREEN_PATH_KEY);
            }
            else
            {
                EditorPrefs.SetString(LAST_SCREEN_PATH_KEY, path);
            }

            IsDirty.Value = false;
        }

        void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            // This runs for every drag tick and every keystroke, so it stays a plain read once the
            // screen is already known to be dirty.
            if (IsDirty.Value)
            {
                return;
            }

            IsDirty.Value = true;
        }
    }
}
