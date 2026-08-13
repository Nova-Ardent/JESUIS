using JESUIS.Shared.ScreenData.Data;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    [System.Serializable]
    public class Screen : ScreenMetaData
    {
        [SerializeReference] RootElement rootElement = new RootElement();

        public RootElement GetRootElement()
        {
            return rootElement;
        }
    }
}
