using JESUIS.Shared.ScreenData.Data;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder
{
    public interface IRendererElement
    {
        /// <summary>
        /// Assigns the element this renderer draws. The non generic entry point exists because the
        /// renderer type is resolved from the runtime type of the data, which the generic
        /// <see cref="IRendererElement{T}.Data"/> cannot be reached through.
        /// </summary>
        public void SetData(BaseElement data);
        public Shared.ScreenData.Types.Transform GetTransform();
        public void OnValuesChanged();
        public void OnParentGeometryChanged(GeometryChangedEvent geometryChangedEvent);
    }

    public interface IRendererElement<T> : IRendererElement where T : BaseElement
    {
        /// <summary>
        /// Read only, so <see cref="IRendererElement.SetData"/> stays the single way the data is
        /// assigned and implementations have only one setter to keep consistent.
        /// </summary>
        public T Data { get; }
    }
}
