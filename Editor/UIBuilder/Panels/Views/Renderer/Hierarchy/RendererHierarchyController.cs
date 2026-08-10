using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy
{
    public class RendererHierarchyController : BaseRendererElement
    {
        BoxSelector boxSelector;
        RendererElementLoader elementLoader = RendererElementLoader.Instance;

        int currentScreenWidth = 0;
        int currentScreenHeight = 0;

        Screen currentScreen;
        Dictionary<BaseElement, VisualElement> elementToRendererElementMap = new Dictionary<BaseElement, VisualElement>();

        public RendererHierarchyController(Screen screen, BoxSelector boxSelector)
        {
            this.boxSelector = boxSelector;
            
            style.position = Position.Absolute;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
            style.left = 0;
            style.right = 0;

            currentScreen = screen;
            elementToRendererElementMap.Add(screen.GetRootElement(), this);
        }

        public void OnChangeAspectRatio(int width, int height)
        {
            currentScreenWidth = width;
            currentScreenHeight = height;

            currentScreen.GetRootElement().Transform.Size.x = width;
            currentScreen.GetRootElement().Transform.Size.y = height;

            RestructureViewElements(currentScreen.GetRootElement());
            boxSelector.WrapToTarget();
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

        public void OnElementIsDirty(ElementChanges elementChanges)
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
            else if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ElementRemoved)
            {
                if (elementChanges is ElementRemoved childRemovedChange)
                {
                    VisualElement targetElement = elementToRendererElementMap[elementChanges.TargetElement];
                    targetElement.parent.Remove(targetElement);
                    elementToRendererElementMap.Remove(elementChanges.TargetElement);
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

        public void OnCurrentScreenChanged(Shared.ScreenData.Screen currentScreen)
        {
            OnSelectedElementChanged(null);
            this.Clear();

            this.currentScreen = currentScreen;
            elementToRendererElementMap.Clear();
            elementToRendererElementMap.Add(currentScreen.GetRootElement(), this);
            
            RebuildHierarchy(currentScreen.GetRootElement(), this);
            schedule.Execute(() =>
            {
                OnChangeAspectRatio(currentScreenWidth, currentScreenHeight);
            });
        }

        void RebuildHierarchy(BaseElement currentData, VisualElement currentVisualElement)
        {
            foreach (BaseElement childData in currentData.GetChildren())
            {
                VisualElement childVisualElement = elementLoader.InstantiateRendererElement(childData);
                currentVisualElement.Add(childVisualElement);

                elementToRendererElementMap[childData] = childVisualElement;

                RebuildHierarchy(childData, childVisualElement);
            }
        }

        void RestructureViewElements(BaseElement baseElement)
        {
            VisualElement visualElement = elementToRendererElementMap[baseElement];
            if (baseElement is not RootElement && visualElement is IRendererElement rendererElement)
            {
                rendererElement.OnValuesChanged();
            }

            foreach (var child in baseElement.GetChildren())
            {
                RestructureViewElements(child);
            }
        }
    } 
}
