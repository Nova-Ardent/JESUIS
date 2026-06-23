using System.Collections.Generic;

namespace JESUIS.Shared.ScreenData.ScreenDataTypes
{
    [System.Serializable]
    public class BaseElement
    {
        protected string Name;

        List<BaseElement> children = new List<BaseElement>();

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
