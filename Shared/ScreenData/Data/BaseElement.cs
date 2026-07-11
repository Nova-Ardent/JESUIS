using JESUIS.Shared.ScreenData.Types;
using System.Collections.Generic;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.Data
{
    [System.Serializable]
    public class BaseElement
    {
        [SerializeField] protected string Name = "";
        [SerializeField] protected Types.Transform Transform = new Types.Transform();

        [SerializeReference] List<BaseElement> children = new List<BaseElement>(); 

        public void AddChild(BaseElement child)
        {
            children.Add(child);
        }

        public void RemoveChild(BaseElement child)
        {
            children.Remove(child);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
