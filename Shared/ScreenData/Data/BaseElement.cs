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

        public BaseElement GetChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                return null;
            }

            return children[index];
        }

        public IEnumerable<BaseElement> GetChildren()
        {
            return children;
        }
        
        public int ChildCount()
        {
            return children.Count;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
