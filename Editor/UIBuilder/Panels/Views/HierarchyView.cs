using JESUIS.Editor.Elements.Common.Widgets;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.Helpers.Utils;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Shared.ScreenData.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class HierarchyView : EditorViews
    {
        EditorState editorState;
        HierarchyItem hierarchyItem;

        public override Views Type { get => Views.Hierarchy; }

        Elements.Common.Widgets.Hierarchy editorHierarchy;

        public HierarchyView(EditorState editorState)
        {
            this.editorState = editorState;
            editorHierarchy = new Elements.Common.Widgets.Hierarchy(editorState.CurrentScreen.GetRootElement(), GetActions, OnElementClicked);
            editorState.ListenToSelectedElementIsDirty(OnSelectedElementIsDirty);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);

            Add(editorHierarchy);
        }

        void OnElementClicked(HierarchyItem item)
        {
            if (item.TargetObject is BaseElement baseElement)
            {
                hierarchyItem = item;
                editorState.SelectedElement.Value = baseElement;
            }
        }

        void OnSelectedElementIsDirty(EditorViews triggeringView)
        {
            if (triggeringView == this)
                return;

            hierarchyItem.UpdateLabel();
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
                item.AddChild(new HierarchyItem(newEmpty, GetActions, OnElementClicked));
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
