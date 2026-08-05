using JESUIS.Editor.Elements.Layout.TabBarWidgets;
using System.Collections.Generic;
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

        public virtual IEnumerable<TabElement> GetActiveTabOptions()
        {
            yield break;
        }
    }
}
