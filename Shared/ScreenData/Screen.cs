using JESUIS.Shared.ScreenData.Data;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    [System.Serializable]
    public class Screen : ScriptableObject
    {
        [SerializeReference] RootElement rootElement = new RootElement();

        public RootElement GetRootElement()
        {
            // A managed reference deserializes to null when the asset was written before the
            // field was serialized, and every view walks the tree from here.
            rootElement ??= new RootElement();
            return rootElement;
        }
    }
}
