using UnityEditor;
using System;
using JESUIS.Shared.ScreenData;
using System.IO;

namespace JESUIS.Editor.Utilities
{
    public class AssetPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string moveAsset in movedAssets)
            {
                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(moveAsset);
                if (assetType == typeof(ScreenMetaData))
                {
                    ScreenMetaData screenMetaData = AssetDatabase.LoadAssetAtPath<ScreenMetaData>(moveAsset);
                    string newPath = Path.GetDirectoryName(moveAsset);

                    if (screenMetaData.TryUpdatePath(newPath))
                    {
                        EditorUtility.SetDirty(screenMetaData);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    if (EditorUtility.IsPersistent(screenMetaData))
                        UnityEngine.Resources.UnloadAsset(screenMetaData);
                    else
                        UnityEngine.Object.DestroyImmediate(screenMetaData, true);
                }
            }
        }
    }
}
