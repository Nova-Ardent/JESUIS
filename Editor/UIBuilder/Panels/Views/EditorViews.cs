using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class EditorViews : VisualElement
    {
        public virtual Views Type { get => Views.None; }

        public enum Views
        {
            None,
            Hierarchy,
            Inspector,
            Renderer,
        }
    }
}
