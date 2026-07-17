using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Shared.ScreenData.Data;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy
{
    public class RendererHierarchyController : BaseRendererElement
    {
        BoxSelector boxSelector;
        EditorState editorState;
        RendererElementLoader elementLoader = RendererElementLoader.Instance;

        Dictionary<BaseElement, VisualElement> elementToRendererElementMap = new Dictionary<BaseElement, VisualElement>();

        public RendererHierarchyController(EditorState editorState, BoxSelector boxSelector)
        {
            this.boxSelector = boxSelector;
            this.editorState = editorState;
            this.editorState.ListenToElementIsDirty(OnElementIsDirty);
            
            style.position = Position.Absolute;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
            style.left = 0;
            style.right = 0;

            elementToRendererElementMap.Add(editorState.CurrentScreen.GetRootElement(), this);
            editorState.SelectedElement.ListenTo(OnSelectedElementChanged);
        }

        public void OnSelectedElementChanged(BaseElement selectedElement)
        {
            if (selectedElement == null || selectedElement is RootElement)
            {
                boxSelector.SetActive(false);
            }
            else
            {
                boxSelector.SetActive(true);
                boxSelector.SetTarget(elementToRendererElementMap[selectedElement]);
                boxSelector.BringToFront();
            }
        }

        public void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ChildAdded)
            {
                if (elementChanges is ChildAdded childAddedChange)
                {
                    VisualElement childElement = elementLoader.InstantiateRendererElement(childAddedChange.Data);
                    VisualElement targetElement = elementToRendererElementMap[elementChanges.TargetElement];
                    targetElement.Add(childElement);

                    if (childElement is IRendererElement rendererElement)
                    {
                        rendererElement.OnValuesChanged();
                        targetElement.RegisterCallback<GeometryChangedEvent>(rendererElement.OnParentGeometryChanged);
                    }

                    elementToRendererElementMap[childAddedChange.Data] = childElement;
                }
            }
            else if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ValueUpdated)
            {
                if (elementChanges is ValuesUpdated valuesUpdatedChange)
                {
                    VisualElement targetElement = elementToRendererElementMap[valuesUpdatedChange.TargetElement];
                    if (targetElement is IRendererElement rendererElement)
                    {
                        rendererElement.OnValuesChanged();
                        boxSelector.WrapToTarget();
                    }
                }
            }
        }
    }
}
