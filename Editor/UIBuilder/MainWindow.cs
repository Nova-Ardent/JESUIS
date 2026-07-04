using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Elements.Window;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Panels;
using JESUIS.Editor.UIBuilder.Panels.Views;
using System.Collections.Generic;
using UnityEditor;

namespace JESUIS.Editor.UIBuilder
{
    public class MainWindow : BaseWindow<MainWindow>
    {
        EditorState editorState = new EditorState();

        UIEditorLayoutManager uIEditorLayoutManager;
        ViewManager viewManager;

        SplittablePanel mainPanel;

        [MenuItem("JESUIS/UI Builder")]
        public static void ShowWindow()
        {
            LaunchWindow("JESUIS");
        }

        protected override void CreateGUI()
        {
            editorState.CurrentScreen = CreateInstance<Shared.ScreenData.Screen>();

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
        }

        private void OnInspectorUpdate()
        {
            uIEditorLayoutManager.SaveLayout();
        }

        protected override IEnumerable<NamedAction> GetContextMenuOptions()
        {
            yield return new NamedAction("Split Vertically", () => mainPanel.SplitVertically(new UIEditorPanel(viewManager, uIEditorLayoutManager)), true);
            yield return new NamedAction("Split Horizontally", () => mainPanel.SplitHorizontally(new UIEditorPanel(viewManager, uIEditorLayoutManager)), true);
        }
    }
}
