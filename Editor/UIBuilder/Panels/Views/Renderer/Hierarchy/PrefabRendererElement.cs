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

        public override void OnValuesChanged()
        {
            PrefabElement prefabElement = (PrefabElement)base.Data;
            
            if (lastPrefab != prefabElement.Prefab.Prefab)
            {
                lastPrefab = prefabElement.Prefab.Prefab;

                Clear();

                if (prefabElement.Prefab.Prefab != null)
                    BuildHierarchy(prefabElement.Prefab.Prefab.GetRootElement(), this);
            }

            base.OnValuesChanged();
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
