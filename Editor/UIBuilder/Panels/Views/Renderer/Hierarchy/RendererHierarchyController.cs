using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Shared.ScreenData.Data;
using System.Collections.Generic;
using UnityEngine.UIElements;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;
using JESUIS.Shared.ScreenData;


namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy
{
    public class RendererHierarchyController : BaseRendererElement
    {
        BoxSelector boxSelector;
        RendererElementLoader elementLoader = RendererElementLoader.Instance;

        Dictionary<BaseElement, VisualElement> elementToRendererElementMap = new Dictionary<BaseElement, VisualElement>();

        public RendererHierarchyController(BoxSelector boxSelector)
        {
            this.boxSelector = boxSelector;

            style.position = Position.Absolute;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
            style.left = 0;
            style.right = 0;
        }

        /// <summary>
        /// Rebuilds the whole renderer tree from <paramref name="screen"/>. Percentage units resolve
        /// against a parent that has not been laid out yet, so sizes stay wrong until the existing
        /// <see cref="GeometryChangedEvent"/> chain corrects them on the next layout pass.
        /// </summary>
        public void SetScreen(Screen screen)
        {
            ClearRendererElements();

            if (screen == null)
            {
                Data = null;
                return;
            }

            Data = screen.GetRootElement();
            elementToRendererElementMap.Add(Data, this);

            BuildSubtree(Data, this);
        }

        /// <summary>
        /// The controller always fills the render surface, so it deliberately ignores the root
        /// element's transform rather than styling itself from it like every other renderer element.
        /// </summary>
        public override void OnValuesChanged()
        {
        }

        public void OnSelectedElementChanged(BaseElement selectedElement)
        {
            if (selectedElement == null || selectedElement is RootElement || !elementToRendererElementMap.TryGetValue(selectedElement, out VisualElement rendererElement))
            {
                boxSelector.SetActive(false);
                return;
            }

            boxSelector.SetActive(true);
            boxSelector.SetTarget(rendererElement);
            boxSelector.BringToFront();
        }

        public void OnElementIsDirty(ElementChanges elementChanges)
        {
            if (elementChanges is ChildAdded childAddedChange)
            {
                if (elementToRendererElementMap.TryGetValue(childAddedChange.TargetElement, out VisualElement parentElement))
                {
                    AttachRendererElement(childAddedChange.Data, parentElement);
                }
            }
            else if (elementChanges is ChildRemoved childRemovedChange)
            {
                DetachRendererElement(childRemovedChange.Data);
            }
            else if (elementChanges is ValuesUpdated valuesUpdatedChange)
            {
                if (elementToRendererElementMap.TryGetValue(valuesUpdatedChange.TargetElement, out VisualElement targetElement) && targetElement is IRendererElement rendererElement)
                {
                    rendererElement.OnValuesChanged();

                    if (boxSelector.GetTarget() == targetElement)
                    {
                        boxSelector.WrapToTarget();
                    }
                }
            }
        }

        void BuildSubtree(BaseElement data, VisualElement parentElement)
        {
            foreach (BaseElement child in data.GetChildren())
            {
                VisualElement childElement = AttachRendererElement(child, parentElement);
                if (childElement == null)
                {
                    continue;
                }

                BuildSubtree(child, childElement);
            }
        }

        VisualElement AttachRendererElement(BaseElement data, VisualElement parentElement)
        {
            VisualElement childElement = elementLoader.InstantiateRendererElement(data);
            if (childElement == null)
            {
                return null;
            }

            parentElement.Add(childElement);

            if (childElement is IRendererElement rendererElement)
            {
                rendererElement.OnValuesChanged();
                parentElement.RegisterCallback<GeometryChangedEvent>(rendererElement.OnParentGeometryChanged);
            }

            elementToRendererElementMap[data] = childElement;
            return childElement;
        }

        void DetachRendererElement(BaseElement data)
        {
            foreach (BaseElement element in data.EnumerateSubtree())
            {
                if (!elementToRendererElementMap.TryGetValue(element, out VisualElement rendererElement))
                {
                    continue;
                }

                // The callback has to go before the element leaves the tree, both because the parent
                // is needed to unregister it and because it would otherwise fire on a parentless
                // element.
                if (rendererElement.parent != null && rendererElement is IRendererElement renderer)
                {
                    rendererElement.parent.UnregisterCallback<GeometryChangedEvent>(renderer.OnParentGeometryChanged);
                }

                rendererElement.RemoveFromHierarchy();
                elementToRendererElementMap.Remove(element);
            }
        }

        void ClearRendererElements()
        {
            // Only the direct children registered their callback on this controller, which outlives
            // the swap; every deeper registration sits on a parent that is discarded with the
            // subtree, so the whole tree can go in one Clear rather than node by node.
            if (Data != null)
            {
                foreach (BaseElement child in Data.GetChildren())
                {
                    if (elementToRendererElementMap.TryGetValue(child, out VisualElement rendererElement) && rendererElement is IRendererElement renderer)
                    {
                        UnregisterCallback<GeometryChangedEvent>(renderer.OnParentGeometryChanged);
                    }
                }
            }

            elementToRendererElementMap.Clear();
            Clear();
        }
    }
}
