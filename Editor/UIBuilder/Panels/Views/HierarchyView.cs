using JESUIS.Editor.Elements.Widgets;
using JESUIS.Editor.Helpers;
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
            editorHierarchy = new Elements.Widgets.Hierarchy(editorState.CurrentScreen.GetRootElement(), GetActions, OnElementClicked);
            editorState.ListenToElementIsDirty(OnElementIsDirty);

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

        void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (triggeringView == this)
                return;

            if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ValueUpdated)
            {
                hierarchyItem.UpdateLabel();
            }
        }

        IEnumerable<NamedAction> GetActions(HierarchyItem item)
        {
            if (item.TargetObject is BaseElement)
            {
                yield return new NamedAction("Add Empty", () => AddEmpty(item), true);
                yield return new NamedAction("Add Texture", () => AddTexture(item), true);
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

        void AddTexture(HierarchyItem item)
        {
            TextureElement newTexture = new TextureElement();
            newTexture.SetName("New Texture");

            if (item.TargetObject is BaseElement baseElement)
            {
                baseElement.AddChild(newTexture);
                item.AddChild(new HierarchyItem(newTexture, GetActions, OnElementClicked));
                editorHierarchy.RebuildListVisuals();
                editorState.TriggerElementIsDirty(this, new ChildAdded(baseElement, newTexture));
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

                item.Remove();
                editorHierarchy.RebuildListVisuals();
                editorState.TriggerElementIsDirty(this, new ElementRemoved(baseElement));
                editorState.SelectedElement.Value = null;
            }
        }
    }
}
