using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using System.IO;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class FileView : EditorViews
    {
        const string UnsavedScreenID = "Unsaved Screen";

        ObjectFieldElement<Shared.ScreenData.Screen> currentFile = new ObjectFieldElement<Shared.ScreenData.Screen>("Current Screen");
        TextInputFieldElement currentFileUID = new TextInputFieldElement("Id: ", UnsavedScreenID);

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

            currentFile.SetValueWithoutNotify(editorState.CurrentScreen);
            currentFile.RegisterOnValueChanged(screen =>
            {
                if (screen == null)
                {
                    screen = ScriptableObject.CreateInstance<Shared.ScreenData.Screen>();
                    screen.name = "Unsaved Screen";
                    screen.Uid = Guid.NewGuid().ToString("N");
                    currentFileUID.SetValueWithoutNotify(screen.Uid);
                    currentFile.SetValueWithoutNotify(screen);
                }
                else
                {
                    currentFileUID.SetValueWithoutNotify(screen.Uid);
                }

                if (CurrentEditorState.CurrentScreen.Value != null)
                {
                    if (EditorUtility.IsPersistent(CurrentEditorState.CurrentScreen.Value))
                        UnityEngine.Resources.UnloadAsset(CurrentEditorState.CurrentScreen.Value);
                    else
                        UnityEngine.Object.DestroyImmediate(CurrentEditorState.CurrentScreen.Value, true);
                }
                CurrentEditorState.CurrentScreen.Value = screen;
            });
            Add(currentFile);

            currentFileUID.SetValueWithoutNotify(editorState.CurrentScreen.Value.Uid); 
            Add(currentFileUID); 
            
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
            if (currentFile.CurrentValue == null)
                return;

            string assetPath = AssetDatabase.GetAssetPath(currentFile.CurrentValue);
            if (string.IsNullOrEmpty(assetPath) || !File.Exists(assetPath))
            {
                SaveAsButtonClicked();
                return;
            }

            EditorUtility.SetDirty(currentFile.CurrentValue);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void SaveAsButtonClicked()
        {
            if (currentFile.CurrentValue == null)
                return;

            string path = EditorUtility.SaveFilePanelInProject(
                "Save ScriptableObject",
                currentFile.CurrentValue.name,
                "asset",
                "Choose where to save the asset.");

            if (string.IsNullOrEmpty(path))
                return;

            ScriptableObject asset = currentFile.CurrentValue;

            if (AssetDatabase.Contains(asset))
            {
                asset = UnityEngine.Object.Instantiate(asset);
            }

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentFile.SetValue((Shared.ScreenData.Screen)asset);
        }
    }
}
