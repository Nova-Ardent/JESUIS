using System;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    public class ScreenMetaData : ScriptableObject
    {
        [SerializeField] public string Uid;


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
