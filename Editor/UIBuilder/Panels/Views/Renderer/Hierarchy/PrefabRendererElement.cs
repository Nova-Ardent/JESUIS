using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData;
using UnityEngine.UIElements;

namespace Assets.JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy
{
    [RendererElement(typeof(PrefabElement))]
    public class PrefabRendererElement : EmptyRendererElement, IRendererElement<PrefabElement>
    {
        VisualElement prefabContainer = new VisualElement();

        Screen lastPrefab;
        RendererElementLoader elementLoader = RendererElementLoader.Instance;

        PrefabElement IRendererElement<PrefabElement>.Data
        {
            get
            {
                return (PrefabElement)base.Data;
            }
            set
            {
                base.Data = value;
            }
        }

        public PrefabRendererElement() : base()
        {
            prefabContainer.style.left = 0;
            prefabContainer.style.top = 0;
            prefabContainer.style.position = Position.Absolute;
            Add(prefabContainer);
        }

        public override void OnValuesChanged()
        {
            PrefabElement prefabElement = (PrefabElement)base.Data;
            
            if (lastPrefab != prefabElement.Prefab.Prefab)
            {
                lastPrefab = prefabElement.Prefab.Prefab;

                prefabContainer.Clear();

                if (prefabElement.Prefab.Prefab != null)
                    BuildHierarchy(prefabElement.Prefab.Prefab.GetRootElement(), prefabContainer);
            }

            base.OnValuesChanged();
            prefabContainer.style.width = this.style.width;
            prefabContainer.style.height = this.style.height;
            UpdateChildValues(this);
        }

        void BuildHierarchy(BaseElement currentData, VisualElement currentVisualElement)
        {
            foreach (BaseElement childData in currentData.GetChildren())
            {
                VisualElement childVisualElement = elementLoader.InstantiateRendererElement(childData);
                currentVisualElement.Add(childVisualElement);
                BuildHierarchy(childData, childVisualElement);
            }
        }

        void UpdateChildValues(VisualElement visualElement)
        {
            foreach (var child in visualElement.Children())
            {
                if (child is IRendererElement rendererElement)
                {
                    rendererElement.OnValuesChanged();
                }

                UpdateChildValues(child);
            }
        }
    }
}
