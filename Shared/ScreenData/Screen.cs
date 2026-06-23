using JESUIS.Shared.ScreenData.ScreenDataTypes;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    [System.Serializable]
    public class Screen : ScriptableObject
    {
        RootElement rootElement = new RootElement();

        public RootElement GetRootElement()
        {
            return rootElement;
        }
    }
}
