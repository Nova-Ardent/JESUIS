using UnityEngine.UIElements;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Shared.ScreenData.Data;
using UnityEngine;
using JESUIS.Shared.ScreenData.Types;

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
            style.backgroundColor = new Color(0.5f, 0, 0, 0.5f);
            style.position = Position.Absolute;
            style.width = GetHorizontal(Data.Transform.Size.x, Data.Transform.HorizontalSize);
            style.height = GetVertical(Data.Transform.Size.y, Data.Transform.VerticalSize);

            Vector2 pivotOffset = GetPivotOffset();
            Vector2 position = GetAnchorOffset() - pivotOffset;
            style.left = position.x;
            style.top = position.y;

            style.transformOrigin = new TransformOrigin(pivotOffset.x, pivotOffset.y, 0);

            style.rotate = new Rotate(new Angle(Data.Transform.Rotation, AngleUnit.Degree));
            style.scale = new Scale(new Vector3(Data.Transform.Scale.x, Data.Transform.Scale.y, 1));
        }

        public void OnParentGeometryChanged(GeometryChangedEvent geometryChangedEvent)
        {
            OnValuesChanged();
        }

        float GetHorizontal(float size, Unit unit)
        {
            return unit == Unit.Pixels ? size : parent.contentRect.width * size / 100;
        }

        float GetVertical(float size , Unit unit)
        {
            return unit == Unit.Pixels ? size : parent.contentRect.height * size / 100;
        }

        Vector2 GetAnchorOffset()
        {
            float posX = GetHorizontal(Data.Transform.Position.x, Data.Transform.HorizontalPosition);
            float posY = GetVertical(Data.Transform.Position.y, Data.Transform.VerticalPosition);

            switch (Data.Transform.Anchor)
            {
                default:
                    break;
                case Alignment.Top:
                case Alignment.Middle:
                case Alignment.Bottom:
                    posX += GetHorizontal(50, Unit.Percentage);
                    break;

                case Alignment.TopRight:
                case Alignment.Right:
                case Alignment.BottomRight:
                    posX += GetHorizontal(100, Unit.Percentage);
                    break;
            }

            switch (Data.Transform.Anchor)
            {
                default:
                    break;
                case Alignment.Left:
                case Alignment.Middle:
                case Alignment.Right:
                    posY += GetVertical(50, Unit.Percentage);
                    break;
                case Alignment.BottomLeft:
                case Alignment.Bottom:
                case Alignment.BottomRight:
                    posY += GetVertical(100, Unit.Percentage);
                    break;
            }

            return new Vector2(posX, posY);
        }

        Vector2 GetPivotOffset()
        {
            float posX = 0;
            float posY = 0;

            switch (Data.Transform.Pivot)
            {
                default:
                    break;
                case Alignment.Top:
                case Alignment.Middle:
                case Alignment.Bottom:
                    posX += style.width.value.value / 2;
                    break;

                case Alignment.TopRight:
                case Alignment.Right:
                case Alignment.BottomRight:
                    posX += style.width.value.value;
                    break;
            }

            switch (Data.Transform.Pivot)
            {
                default:
                    break;
                case Alignment.Left:
                case Alignment.Middle:
                case Alignment.Right:
                    posY += style.height.value.value / 2;
                    break;
                case Alignment.BottomLeft:
                case Alignment.Bottom:
                case Alignment.BottomRight:
                    posY += style.height.value.value;
                    break;
            }

            return new Vector2(posX, posY);
        }

        public Shared.ScreenData.Types.Transform GetTransform()
        {
            return Data.Transform;
        }
    }
}
