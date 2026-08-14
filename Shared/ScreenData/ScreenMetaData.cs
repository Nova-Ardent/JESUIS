using System;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    [System.Serializable]
    public class ScreenMetaData : ScriptableObject
    {
        [SerializeField] public string Uid;

        public void Initialize()
        {
            Uid = Guid.NewGuid().ToString("N");
        }

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
    }
}
