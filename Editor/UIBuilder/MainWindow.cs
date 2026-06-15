using JESUIS.Editor.Elements.Common.Layout;
using JESUIS.Editor.Elements.Common.Window;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.UIBuilder.Panels;
using JESUIS.Editor.UIBuilder.Panels.Views;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace JESUIS.Editor.UIBuilder
{
    public class MainWindow : BaseWindow<MainWindow>
    {
        SplittablePanel mainPanel;

        [MenuItem("JESUIS/UI Builder")]
        public static void ShowWindow()
        {
            LaunchWindow("JESUIS");
        }

        protected override void CreateGUI()
        {
            mainPanel = new SplittablePanel();
            mainPanel.SetToInitialState(new UIEditorPanel());

            rootVisualElement.Add(mainPanel);
            base.CreateGUI();
        }

        protected override IEnumerable<NamedAction> GetContextMenuOptions()
        {
            yield return new NamedAction("Split Vertically", () => mainPanel.SplitVertically(new UIEditorPanel()), true);
            yield return new NamedAction("Split Horizontally", () => mainPanel.SplitHorizontally(new UIEditorPanel()), true);
        }
    }
}
