using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class FileView : EditorViews
    {
        ObjectFieldElement<Shared.ScreenData.Screen> CurrentFile = new ObjectFieldElement<Shared.ScreenData.Screen>("Current Screen");
        ButtonElement saveButton = new ButtonElement("Save");
        ButtonElement saveAsButton = new ButtonElement("Save As");

        VisualElement saveButtonContainer = new VisualElement();

        public override Views Type => Views.File;

        public FileView(EditorState editorState) : base(editorState)
        {
            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);

            CurrentFile.SetValueWithoutNotify(editorState.CurrentScreen);
            CurrentFile.RegisterOnValueChanged(screen =>
            {
                if (screen == null)
                {
                    screen = ScriptableObject.CreateInstance<Shared.ScreenData.Screen>();
                    screen.name = "Unsaved Screen";
                    CurrentFile.SetValueWithoutNotify(screen);
                } 

                CurrentEditorState.CurrentScreen.Value = screen;
            });
            Add(CurrentFile);

            saveButtonContainer.style.flexDirection = FlexDirection.Row;
            Add(saveButtonContainer);

            saveButton.style.width = Length.Percent(48);
            saveButton.clickable.clicked += SaveButtonClicked;
            saveButtonContainer.Add(saveButton);

            saveAsButton.style.width = Length.Percent(48);
            saveAsButton.clickable.clicked += SaveAsButtonClicked;
            saveButtonContainer.Add(saveAsButton);
        }

        protected override void OnElementIsDirty(EditorViews editorViews, ElementChanges elementChanges)
        {
        }

        void SaveButtonClicked()
        {
            if (CurrentFile.CurrentValue == null)
                return;

            string assetPath = AssetDatabase.GetAssetPath(CurrentFile.CurrentValue);
            if (string.IsNullOrEmpty(assetPath) || !File.Exists(assetPath))
            {
                SaveAsButtonClicked();
                return;
            }

            EditorUtility.SetDirty(CurrentFile.CurrentValue);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void SaveAsButtonClicked()
        {
            if (CurrentFile.CurrentValue == null)
                return;

            string path = EditorUtility.SaveFilePanelInProject(
                "Save ScriptableObject",
                CurrentFile.CurrentValue.name,
                "asset",
                "Choose where to save the asset.");

            if (string.IsNullOrEmpty(path))
                return;

            ScriptableObject asset = CurrentFile.CurrentValue;

            if (AssetDatabase.Contains(asset))
            {
                asset = Object.Instantiate(asset);
            }

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            CurrentFile.SetValue((Shared.ScreenData.Screen)asset);
        }
    }
}
