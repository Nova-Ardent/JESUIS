using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Shared.ScreenData.Data;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy
{
    [RendererElement(typeof(BaseElement))]
    public class BaseRendererElement : VisualElement, IRendererElement<BaseElement>
    {
        public BaseElement Data { get; set; }

        public BaseRendererElement()
        {
        }

        public virtual void OnValuesChanged()
        {
            style.position = Position.Absolute;

            Vector2 localPosition = Data.Transform.GetLocalPosition();
            style.translate = localPosition;

            style.width = new Length(Data.Transform.GetLocalUnitWidth());
            style.height = new Length(Data.Transform.GetLocalUnitHeight());

            Vector2 pivot = Data.Transform.GetPivotOffset();
            style.transformOrigin = new TransformOrigin(pivot.x, pivot.y, 0);

            style.rotate = new Rotate(new Angle(Data.Transform.Rotation, AngleUnit.Degree));
            style.scale = new Scale(new Vector3(Data.Transform.Scale.x, Data.Transform.Scale.y, 1));
        }

        public void OnParentGeometryChanged(GeometryChangedEvent geometryChangedEvent)
        {
            OnValuesChanged();
        }

        public Shared.ScreenData.Types.Transform GetTransform()
        {
            return Data.Transform;
        }

        public override string ToString()
        {
            return Data.Name;
        }
    }
}
