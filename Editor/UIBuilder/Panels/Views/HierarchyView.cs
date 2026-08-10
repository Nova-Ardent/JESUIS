using JESUIS.Editor.Elements.Widgets;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Shared.ScreenData.Data;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class HierarchyView : EditorViews
    {
        HierarchyItem hierarchyItem;

        public override Views Type { get => Views.Hierarchy; }

        Elements.Widgets.Hierarchy editorHierarchy;

        public HierarchyView(EditorState editorState) : base(editorState)
        {
            editorHierarchy = new Elements.Widgets.Hierarchy(editorState.CurrentScreen.Value.GetRootElement(), GetActions, OnElementClicked);
            Add(editorHierarchy);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);

        }

        void OnElementClicked(HierarchyItem item)
        {
            if (item.TargetObject is BaseElement baseElement)
            {
                hierarchyItem = item;
                CurrentEditorState.SelectedElement.Value = baseElement;
            }
        }

        protected override void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (triggeringView == this)
                return;

            if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ValueUpdated)
            {
                hierarchyItem.UpdateLabel();
            }
        }

        protected override void OnCurrentScreenChanged(Shared.ScreenData.Screen currentScreen)
        {
            Clear();
            editorHierarchy = new Elements.Widgets.Hierarchy(currentScreen.GetRootElement(), GetActions, OnElementClicked);
            Add(editorHierarchy);

            RebuildHierarchy(currentScreen.GetRootElement(), editorHierarchy.RootItem);

            editorHierarchy.RebuildListVisuals();
        }

        void RebuildHierarchy(BaseElement baseElement, HierarchyItem hierarchyItem)
        {
            foreach (BaseElement childElement in baseElement.GetChildren())
            {
                HierarchyItem childHierarchyItem = new HierarchyItem(childElement, GetActions, OnElementClicked);
                hierarchyItem.AddChild(childHierarchyItem);
                RebuildHierarchy(childElement, childHierarchyItem);
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
                CurrentEditorState.TriggerElementIsDirty(this, new ChildAdded(baseElement, newEmpty));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ChildAdded(baseElement, newTexture));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ElementRemoved(baseElement));
                CurrentEditorState.SelectedElement.Value = null;
            }
        }
    }
}
