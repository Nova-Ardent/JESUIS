using JESUIS.Editor.Elements.Common.Layout;
using JESUIS.Editor.Elements.Common.Window;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.UIBuilder.Panels;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace JESUIS.Editor.UIBuilder
{
    public class MainWindow : BaseWindow<MainWindow>
    {
        SplittablePanel _mainPanel;

        [MenuItem("JESUIS/UI Builder")]
        public static void ShowWindow()
        {
            LaunchWindow("JESUIS");
        }

        protected override void CreateGUI()
        {
            _mainPanel = new SplittablePanel();
            _mainPanel.SetToInitialState(new UIEditorPanel());

            rootVisualElement.Add(_mainPanel);
            base.CreateGUI();
        }

        protected override IEnumerable<NamedAction> GetContextMenuOptions()
        {
            yield return new NamedAction("Split Vertically", () => _mainPanel.SplitVertically(new UIEditorPanel()), true);
            yield return new NamedAction("Split Horizontally", () => _mainPanel.SplitHorizontally(new UIEditorPanel()), true);
        }
    }
}
