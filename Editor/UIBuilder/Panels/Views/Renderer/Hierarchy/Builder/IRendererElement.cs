using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;
using JESUIS.Shared.ScreenData.Data;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder
{
    public interface IRendererElement
    {
        public void OnValuesChanged();
        public void OnParentGeometryChanged(GeometryChangedEvent geometryChangedEvent);
    }

    public interface IRendererElement<T> : IRendererElement where T : BaseElement
    {
        public T Data { get; set; }
    }
}
