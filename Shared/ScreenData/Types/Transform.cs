using JESUIS.Shared.ScreenData.DataBindings;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.Types
{
    [System.Serializable]
    public class Transform
    {
        [SerializeReference] public Transform parent;

        public Bindable<Vector2> Size = new Bindable<Vector2>() { Value = new Vector2(100, 100) };
        public Bindable<Vector2> Position = new Bindable<Vector2>() { Value = new Vector2(0, 0) };
        public Bindable<Vector2> Scale = new Bindable<Vector2>() { Value = new Vector2(1, 1) };
        public Bindable<float> Rotation = new Bindable<float>() { Value = 0f };

        public Alignment Anchor;
        public Alignment Pivot;

        public Bindable<Unit> VerticalPosition = new Bindable<Unit>() { Value = Unit.Pixels };
        public Bindable<Unit> VerticalSize = new Bindable<Unit>() { Value = Unit.Pixels };

        public Bindable<Unit> HorizontalPosition = new Bindable<Unit>() { Value = Unit.Pixels };
        public Bindable<Unit> HorizontalSize = new Bindable<Unit>() { Value = Unit.Pixels };

        public Vector2 GetLocalScaledPosition()
        {
            Vector2 pivotOffset = GetLocalScaledPivot();
            Vector2 position = GetAnchorOffset() - pivotOffset;
            return position;
        }

        public Vector2 GetScaledLocalSize()
        {
            return new Vector2(GetLocalUnitWidth() * Scale.Value.x, GetLocalUnitHeight() * Scale.Value.y);
        }

        public Vector2 GetLocalScaledPivot()
        {
            return new Vector2(GetPivotOffset().x * Scale.Value.x, GetPivotOffset().y * Scale.Value.y);
        }


        public Vector2 GetLocalPosition()
        {
            Vector2 pivotOffset = GetPivotOffset();
            Vector2 position = GetAnchorOffset() - pivotOffset;
            return position;
        }

        public float GetLocalUnitPositionX()
        {
            return GetLocalUnitX(Position.Value.x, HorizontalPosition.Value);
        }

        public float GetLocalUnitPositionY()
        {
            return GetLocalUnitY(Position.Value.y, VerticalPosition.Value);
        }

        public float GetLocalUnitWidth()
        {
            return GetLocalUnitX(Size.Value.x, HorizontalSize.Value);
        }

        public float GetLocalUnitHeight()
        {
            return GetLocalUnitY(Size.Value.y, VerticalSize.Value);
        }

        public Vector2 GetAnchorOffset()
        {
            float posX = GetLocalUnitPositionX();
            float posY = GetLocalUnitPositionY();

            switch (Anchor)
            {
                default:
                    break;
                case Alignment.Top:
                case Alignment.Middle:
                case Alignment.Bottom:
                    posX += GetLocalUnitX(50, Unit.Percentage);
                    break;

                case Alignment.TopRight:
                case Alignment.Right:
                case Alignment.BottomRight:
                    posX += GetLocalUnitX(100, Unit.Percentage);
                    break;
            }

            switch (Anchor)
            {
                default:
                    break;
                case Alignment.Left:
                case Alignment.Middle:
                case Alignment.Right:
                    posY += GetLocalUnitY(50, Unit.Percentage);
                    break;
                case Alignment.BottomLeft:
                case Alignment.Bottom:
                case Alignment.BottomRight:
                    posY += GetLocalUnitY(100, Unit.Percentage);
                    break;
            }

            return new Vector2(posX, posY);
        }

        public Vector2 GetPivotOffset()
        {
            float posX = 0;
            float posY = 0;

            switch (Pivot)
            {
                default:
                    break;
                case Alignment.Top:
                case Alignment.Middle:
                case Alignment.Bottom:
                    posX += GetLocalUnitWidth() / 2;
                    break;

                case Alignment.TopRight:
                case Alignment.Right:
                case Alignment.BottomRight:
                    posX += GetLocalUnitWidth();
                    break;
            }

            switch (Pivot)
            {
                default:
                    break;
                case Alignment.Left:
                case Alignment.Middle:
                case Alignment.Right:
                    posY += GetLocalUnitHeight() / 2;
                    break;
                case Alignment.BottomLeft:
                case Alignment.Bottom:
                case Alignment.BottomRight:
                    posY += GetLocalUnitHeight();
                    break;
            }

            return new Vector2(posX, posY);
        }

        float GetLocalUnitX(float position, Unit unit)
        {
            return unit == Unit.Pixels ? position : parent.Size.Value.x * position / 100;
        }

        float GetLocalUnitY(float position, Unit unit)
        {
            return unit == Unit.Pixels ? position : parent.Size.Value.y * position / 100;
        }
    }
}
