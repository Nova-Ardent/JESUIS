using System.Collections.Generic;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.Data
{
    [System.Serializable]
    public class BaseElement
    {
        [SerializeField] public string Name = "";
        [SerializeReference] public Types.Transform Transform = new Types.Transform();

        [SerializeReference] BaseElement parent = null;
        [SerializeReference] List<BaseElement> children = new List<BaseElement>(); 

        public void AddChild(BaseElement child)
        {
            child.parent = this;
            child.Transform.parent = this.Transform;

            children.Add(child);
        }

        public void RemoveChild(BaseElement child)
        {
            children.Remove(child);
        }

        public BaseElement GetParent()
        {
            return parent;
        } 

        public IEnumerable<BaseElement> GetChildren()
        {
            return children;
        } 

        public override string ToString()
        {
            return Name;
        }
    }
}
