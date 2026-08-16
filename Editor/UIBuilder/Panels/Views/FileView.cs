using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using System.IO;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using JESUIS.Shared.ScreenData;

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
                    screen.ScreenMetaData = ScriptableObject.CreateInstance<Shared.ScreenData.ScreenMetaData>();
                    screen.ScreenMetaData.Initialize();

                    currentFileUID.SetValueWithoutNotify(screen.ScreenMetaData.Uid);
                    currentFile.SetValueWithoutNotify(screen);
                }
                else
                {
                    currentFileUID.SetValueWithoutNotify(screen.ScreenMetaData.Uid);
                }

                if (CurrentEditorState.CurrentScreen.Value != null)
                {
                    Shared.ScreenData.Screen.UnloadScreen(CurrentEditorState.CurrentScreen.Value);
                }
                CurrentEditorState.CurrentScreen.Value = screen;
            });
            Add(currentFile);

            currentFileUID.SetValueWithoutNotify(editorState.CurrentScreen.Value.ScreenMetaData.Uid); 
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

            currentFile.CurrentValue.ScreenMetaData.TryUpdatePath(Path.GetDirectoryName(assetPath));
            currentFile.CurrentValue.ScreenMetaData.FileName = Path.GetFileName(assetPath);

            EditorUtility.SetDirty(currentFile.CurrentValue.ScreenMetaData);
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

            string metaDatapath = InsertMetadataSuffix(path);

            JESUIS.Shared.ScreenData.Screen asset = currentFile.CurrentValue; 
            JESUIS.Shared.ScreenData.ScreenMetaData assetMetaData = currentFile.CurrentValue.ScreenMetaData;

            currentFile.CurrentValue.ScreenMetaData.TryUpdatePath(Path.GetDirectoryName(path));
            currentFile.CurrentValue.ScreenMetaData.FileName = Path.GetFileName(path);

            if (AssetDatabase.Contains(asset))
            {
                asset = ScriptableObject.Instantiate(asset);
                assetMetaData = ScriptableObject.Instantiate(assetMetaData);
                assetMetaData.Initialize();
                Debug.LogWarning("New instance of a screen was created, check UID matches for layout loading in game.");

                Shared.ScreenData.Screen.UnloadScreen(currentFile.CurrentValue);
            }

            AssetDatabase.CreateAsset(assetMetaData, metaDatapath);

            asset.ScreenMetaData = assetMetaData;
            AssetDatabase.CreateAsset(asset, path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentFile.SetValue((Shared.ScreenData.Screen)asset);
        }

        static string InsertMetadataSuffix(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            return Path.Combine(directory ?? "", $"{filename}_metadata{extension}");
        }
    }
}
