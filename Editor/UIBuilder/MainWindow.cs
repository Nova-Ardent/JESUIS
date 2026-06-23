using JESUIS.Editor.Elements.Common.Layout;
using JESUIS.Editor.Elements.Common.Window;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.UIBuilder.Panels;
using JESUIS.Editor.UIBuilder.Panels.Views;
using System.Collections.Generic;
using UnityEditor;

namespace JESUIS.Editor.UIBuilder
{
    public class MainWindow : BaseWindow<MainWindow>
    {
        Shared.ScreenData.Screen screenData;

        ViewManager viewManager;
        SplittablePanel mainPanel;

        [MenuItem("JESUIS/UI Builder")]
        public static void ShowWindow()
        {
            LaunchWindow("JESUIS");
        }

        protected override void CreateGUI()
        {
            screenData = new Shared.ScreenData.Screen();
            viewManager = new ViewManager(screenData);

            mainPanel = new SplittablePanel();
            mainPanel.SetToInitialState(new UIEditorPanel(viewManager));

            rootVisualElement.Add(mainPanel);
            base.CreateGUI();
        }

        protected override IEnumerable<NamedAction> GetContextMenuOptions()
        {
            yield return new NamedAction("Split Vertically", () => mainPanel.SplitVertically(new UIEditorPanel(viewManager)), true);
            yield return new NamedAction("Split Horizontally", () => mainPanel.SplitHorizontally(new UIEditorPanel(viewManager)), true);
        }
    }
}
