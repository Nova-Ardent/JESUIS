using JESUIS.Shared.ScreenData.Types;
using System.Collections.Generic;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.Data
{
    [System.Serializable]
    public class BaseElement
    {
        [SerializeField] public string Name = "";
        [SerializeField] public Types.Transform Transform = new Types.Transform();

        [SerializeReference] List<BaseElement> children = new List<BaseElement>(); 

        public void AddChild(BaseElement child)
        {
            children.Add(child);
        }

        public void RemoveChild(BaseElement child)
        {
            children.Remove(child);
        }

        public IReadOnlyList<BaseElement> GetChildren()
        {
            return children;
        }

        /// <summary>
        /// This element followed by every descendant, depth first. Walking the tree is a property of
        /// the data model rather than of any one view, so every recursive consumer shares this.
        /// </summary>
        public IEnumerable<BaseElement> EnumerateSubtree()
        {
            yield return this;

            foreach (BaseElement child in children)
            {
                foreach (BaseElement descendant in child.EnumerateSubtree())
                {
                    yield return descendant;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
