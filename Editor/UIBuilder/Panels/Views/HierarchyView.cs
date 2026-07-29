using JESUIS.Editor.Elements.Widgets;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.Helpers.Utils;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
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

        Elements.Widgets.Hierarchy editorHierarchy;

        public HierarchyView(EditorState editorState)
        {
            this.editorState = editorState;
            editorState.CurrentScreen.ListenTo(OnCurrentScreenChanged);
            editorState.ListenToElementIsDirty(OnElementIsDirty);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
        }

        void OnCurrentScreenChanged(Shared.ScreenData.Screen screen)
        {
            Clear();
            editorHierarchy = null;
            hierarchyItem = null;

            if (screen == null)
            {
                return;
            }

            RootElement rootElement = screen.GetRootElement();

            editorHierarchy = new Elements.Widgets.Hierarchy(rootElement, GetActions, OnElementClicked);
            BuildSubtree(rootElement, editorHierarchy.RootItem);
            editorHierarchy.RebuildListVisuals();

            Add(editorHierarchy);
        }

        void BuildSubtree(BaseElement data, HierarchyItem parentItem)
        {
            foreach (BaseElement child in data.GetChildren())
            {
                HierarchyItem item = new HierarchyItem(child, GetActions, OnElementClicked);
                parentItem.AddChild(item);

                BuildSubtree(child, item);
            }
        }

        void OnElementClicked(HierarchyItem item)
        {
            if (item.TargetObject is BaseElement baseElement)
            {
                hierarchyItem = item;
                editorState.SelectedElement.Value = baseElement;
            }
        }

        void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (triggeringView == this)
                return;

            if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ValueUpdated)
            {
                hierarchyItem?.UpdateLabel();
            }
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
                editorState.TriggerElementIsDirty(this, new ChildAdded(baseElement, newEmpty));
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

                // The selection would otherwise keep pointing into the subtree that just went away.
                if (SubtreeContains(baseElement, editorState.SelectedElement.Value))
                {
                    editorState.SelectedElement.Value = null;
                }

                editorState.TriggerElementIsDirty(this, new ChildRemoved(parentBaseElement, baseElement));
            }

            if (hierarchyItem == item)
            {
                hierarchyItem = null;
            }

            item.Remove();
            editorHierarchy.RebuildListVisuals();
        }

        bool SubtreeContains(BaseElement subtree, BaseElement target)
        {
            if (target == null)
            {
                return false;
            }

            if (subtree == target)
            {
                return true;
            }

            foreach (BaseElement child in subtree.GetChildren())
            {
                if (SubtreeContains(child, target))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
