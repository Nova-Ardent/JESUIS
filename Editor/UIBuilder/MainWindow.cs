using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Elements.Window;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Panels;
using JESUIS.Editor.UIBuilder.Panels.Views;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder
{
    public class MainWindow : BaseWindow<MainWindow>
    {
        const string WINDOW_NAME = "JESUIS";

        EditorState editorState = new EditorState();

        UIEditorLayoutManager uIEditorLayoutManager;
        ScreenAssetManager screenAssetManager;
        ViewManager viewManager;

        SplittablePanel mainPanel;

        [MenuItem("JESUIS/UI Builder")]
        public static void ShowWindow()
        {
            LaunchWindow(WINDOW_NAME);
        }

        protected override void CreateGUI()
        {
            screenAssetManager = new ScreenAssetManager(editorState);
            screenAssetManager.RegisterOnStateChanged(UpdateTitle);

            uIEditorLayoutManager = new UIEditorLayoutManager();
            viewManager = new ViewManager(editorState);
            mainPanel = new SplittablePanel();
            uIEditorLayoutManager.SetRootElement(mainPanel);

            if (!uIEditorLayoutManager.HasSavedLayout())
            {
                mainPanel.SetToInitialState(new UIEditorPanel(viewManager, uIEditorLayoutManager));
            }
            else
            {
                uIEditorLayoutManager.SetRootElement(mainPanel);
                uIEditorLayoutManager.LoadLayout(viewManager);
            }

            rootVisualElement.Add(mainPanel);
            mainPanel.Resize(position.width, position.height);
            base.CreateGUI();

            // The views only build themselves once a screen is broadcast, so this has to come after
            // they exist.
            Shared.ScreenData.Screen screen = screenAssetManager.TryRestoreLastOpened();
            if (screen == null)
            {
                screen = screenAssetManager.CreateNew();
            }

            SetCurrentScreen(screen);
        }

        private void OnInspectorUpdate()
        {
            uIEditorLayoutManager.SaveLayout();
        }

        private void OnDestroy()
        {
            if (screenAssetManager == null || !screenAssetManager.IsDirty)
            {
                return;
            }

            // The window is already going away by the time this runs, so there is no cancel to offer.
            bool save = EditorUtility.DisplayDialog
                ( "Unsaved Screen Changes"
                , $"'{GetCurrentScreenName()}' has unsaved changes. Save them before closing?"
                , "Save"
                , "Discard"
                );

            if (save)
            {
                screenAssetManager.Save(editorState.CurrentScreen.Value);
            }
        }

        public void NewScreen()
        {
            if (!PromptToSaveChanges())
            {
                return;
            }

            SetCurrentScreen(screenAssetManager.CreateNew());
        }

        public void OpenScreen()
        {
            if (!PromptToSaveChanges())
            {
                return;
            }

            Shared.ScreenData.Screen screen = screenAssetManager.Open();
            if (screen == null)
            {
                return;
            }

            SetCurrentScreen(screen);
        }

        public void SaveScreen()
        {
            SetCurrentScreen(screenAssetManager.Save(editorState.CurrentScreen.Value));
        }

        public void SaveScreenAs()
        {
            SetCurrentScreen(screenAssetManager.SaveAs(editorState.CurrentScreen.Value));
        }

        protected override IEnumerable<NamedAction> GetContextMenuOptions()
        {
            yield return new NamedAction("New Screen", NewScreen, true);
            yield return new NamedAction("Open Screen...", OpenScreen, true);
            yield return new NamedAction("Save Screen", SaveScreen, true);
            yield return new NamedAction("Save Screen As...", SaveScreenAs, true);
            yield return new NamedAction("Split Vertically", () => mainPanel.SplitVertically(new UIEditorPanel(viewManager, uIEditorLayoutManager)), true);
            yield return new NamedAction("Split Horizontally", () => mainPanel.SplitHorizontally(new UIEditorPanel(viewManager, uIEditorLayoutManager)), true);
        }

        /// <summary>
        /// Swaps in a newly created, opened or saved screen. Saving hands back the same instance it
        /// was given, so the selection only resets when the screen genuinely changed.
        /// </summary>
        void SetCurrentScreen(Shared.ScreenData.Screen screen)
        {
            if (screen == null || editorState.CurrentScreen.Value == screen)
            {
                UpdateTitle();
                return;
            }

            // Must come first: the views index their lookups by element, and a selection left
            // pointing into the outgoing tree survives the rebuild that follows.
            editorState.SelectedElement.Value = null;
            editorState.CurrentScreen.Value = screen;

            UpdateTitle();
        }

        /// <summary>
        /// Returns false when the caller should abandon whatever it was about to do.
        /// </summary>
        bool PromptToSaveChanges()
        {
            if (!screenAssetManager.IsDirty)
            {
                return true;
            }

            int choice = EditorUtility.DisplayDialogComplex
                ( "Unsaved Screen Changes"
                , $"'{GetCurrentScreenName()}' has unsaved changes."
                , "Save"
                , "Cancel"
                , "Discard"
                );

            switch (choice)
            {
                case 0: return screenAssetManager.Save(editorState.CurrentScreen.Value) != null;
                case 2: return true;
                default: return false;
            }
        }

        void UpdateTitle()
        {
            string dirtyMarker = screenAssetManager.IsDirty ? "*" : "";
            titleContent = new GUIContent($"{WINDOW_NAME} - {GetCurrentScreenName()}{dirtyMarker}");
        }

        string GetCurrentScreenName()
        {
            Shared.ScreenData.Screen screen = editorState.CurrentScreen.Value;
            return screen == null ? "" : screen.name;
        }

        [Shortcut("JESUIS/Save Screen", typeof(MainWindow), KeyCode.S, ShortcutModifiers.Action)]
        static void SaveScreenShortcut(ShortcutArguments arguments)
        {
            if (arguments.context is MainWindow window)
            {
                window.SaveScreen();
            }
        }

        [MenuItem("JESUIS/Screen/New")]
        static void NewScreenMenuItem()
        {
            GetOpenWindow()?.NewScreen();
        }

        [MenuItem("JESUIS/Screen/Open...")]
        static void OpenScreenMenuItem()
        {
            GetOpenWindow()?.OpenScreen();
        }

        [MenuItem("JESUIS/Screen/Save")]
        static void SaveScreenMenuItem()
        {
            GetOpenWindow()?.SaveScreen();
        }

        [MenuItem("JESUIS/Screen/Save As...")]
        static void SaveScreenAsMenuItem()
        {
            GetOpenWindow()?.SaveScreenAs();
        }

        [MenuItem("JESUIS/Screen/New", true)]
        [MenuItem("JESUIS/Screen/Open...", true)]
        [MenuItem("JESUIS/Screen/Save", true)]
        [MenuItem("JESUIS/Screen/Save As...", true)]
        static bool ValidateScreenMenuItem()
        {
            return HasOpenInstances<MainWindow>();
        }

        /// <summary>
        /// Never opens the window, so the screen actions cannot be reached before there is anything
        /// to act on.
        /// </summary>
        static MainWindow GetOpenWindow()
        {
            return HasOpenInstances<MainWindow>() ? GetWindow<MainWindow>() : null;
        }
    }
}
