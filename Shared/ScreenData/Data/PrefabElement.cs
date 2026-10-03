using JESUIS.Shared.ScreenData.Types;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.Data
{
    [System.Serializable]
    [TransformElementLock(TransformDatas.Size)]
    public class PrefabElement : EmptyElement
    {
        [System.Serializable]
        public class PrefabData
        {
            [SerializeReference] public Screen Prefab;
        }

        [SerializeField] public PrefabData Prefab = new PrefabData();

        public override bool PostValueUpdated()
        {
            if (Prefab.Prefab != null)
            {
                Vector2 newSize = Prefab.Prefab.GetRootElement().Transform.Size.Value;
                if (Transform.Size.Value != newSize)
                {
                    Transform.Size.Value = newSize;
                    return true;
                }
            }

            return false;
        }
    }
}
