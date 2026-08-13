using JESUIS.Shared.ScreenData.Data;
using System;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    [System.Serializable]
    public class Screen : ScriptableObject
    {
        [SerializeField] public string Uid;
        [SerializeReference] RootElement rootElement = new RootElement();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Uid))
            {
                Uid = Guid.NewGuid().ToString("N");
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        public RootElement GetRootElement()
        {
            return rootElement;
        }
    }
}
