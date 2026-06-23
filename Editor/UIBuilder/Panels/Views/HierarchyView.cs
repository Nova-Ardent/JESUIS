using JESUIS.Editor.Elements.Common.Widgets;
using JESUIS.Editor.Helpers;
using JESUIS.Shared.ScreenData.ScreenDataTypes;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class HierarchyView : EditorViews
    {
        Shared.ScreenData.Screen screen;

        public override Views Type { get => Views.Hierarchy; }

        Elements.Common.Widgets.Hierarchy editorHierarchy;

        public HierarchyView(Shared.ScreenData.Screen screen)
        {
            this.screen = screen;
            editorHierarchy = new Elements.Common.Widgets.Hierarchy(screen.GetRootElement(), GetActions);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);

            Add(editorHierarchy);
        }

        IEnumerable<NamedAction> GetActions(HierarchyItem item)
        {
            if (item.TargetObject is BaseElement)
            {
                yield return new NamedAction("Add Empty", () => AddEmpty(item), true);
            }

            if (item.TargetObject is BaseElement && item.TargetObject is not RootElement)
            {
                yield return new NamedAction("Remove", () => RemoveElement(item), true);
            }
        }

        void AddEmpty(HierarchyItem item)
        {
            EmptyElement newEmpty = new EmptyElement();
            newEmpty.SetName("New Empty");

            if (item.TargetObject is BaseElement baseElement)
            {
                baseElement.AddChild(newEmpty);
                item.AddChild(new HierarchyItem(newEmpty, GetActions));
                editorHierarchy.RebuildListVisuals();
            }
            else
            {
                Debug.LogError("Target object is not a BaseElement.");
            }
        }

        void RemoveElement(HierarchyItem item)
        {
            if (item.Parent.TargetObject is BaseElement parentBaseElement && item.TargetObject is BaseElement baseElement)
            {
                parentBaseElement.RemoveChild(baseElement);
            }

            item.Remove();
            editorHierarchy.RebuildListVisuals();
        }
    }
}
