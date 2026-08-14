using JESUIS.Shared.ScreenData.Data;
using UnityEditor;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    [System.Serializable]
    public class Screen : ScriptableObject
    {
        [SerializeReference] public ScreenMetaData ScreenMetaData;

        [SerializeReference] RootElement rootElement = new RootElement();

        public RootElement GetRootElement()
        {
            return rootElement;
        }

        public static void UnloadScreen(Screen screen)
        {
            if (EditorUtility.IsPersistent(screen.ScreenMetaData))
                UnityEngine.Resources.UnloadAsset(screen.ScreenMetaData);
            else
                UnityEngine.Object.DestroyImmediate(screen.ScreenMetaData, true);


            if (EditorUtility.IsPersistent(screen))
                UnityEngine.Resources.UnloadAsset(screen);
            else
                UnityEngine.Object.DestroyImmediate(screen, true);
        }
    }
}
