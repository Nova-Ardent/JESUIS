using UnityEngine;

namespace JESUIS.Shared.ScreenData.Types
{
    [System.Serializable]
    public class Transform
    {
        [SerializeReference] public Transform parent;

        public Vector2 Size = new Vector2(100, 100);
        public Vector2 Position;
        public Vector2 Scale = new Vector2(1, 1);
        public float Rotation;

        public Alignment Anchor;
        public Alignment Pivot;

        public Unit VerticalPosition;
        public Unit VerticalSize;

        public Unit HorizontalPosition;
        public Unit HorizontalSize;

        public Vector2 GetLocalScaledPosition()
        {
            Vector2 pivotOffset = GetLocalScaledPivot();
            Vector2 position = GetAnchorOffset() - pivotOffset;
            return position;
        }

        public Vector2 GetScaledLocalSize()
        {
            return new Vector2(GetLocalUnitWidth() * Scale.x, GetLocalUnitHeight() * Scale.y);
        }

        public Vector2 GetLocalScaledPivot()
        {
            return new Vector2(GetPivotOffset().x * Scale.x, GetPivotOffset().y * Scale.y);
        }


        public Vector2 GetLocalPosition()
        {
            Vector2 pivotOffset = GetPivotOffset();
            Vector2 position = GetAnchorOffset() - pivotOffset;
            return position;
        }

        public float GetLocalUnitPositionX()
        {
            return GetLocalUnitX(Position.x, HorizontalPosition);
        }

        public float GetLocalUnitPositionY()
        {
            return GetLocalUnitY(Position.y, VerticalPosition);
        }

        public float GetLocalUnitWidth()
        {
            return GetLocalUnitX(Size.x, HorizontalSize);
        }

        public float GetLocalUnitHeight()
        {
            return GetLocalUnitY(Size.y, VerticalSize);
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
            return unit == Unit.Pixels ? position : parent.Size.x * position / 100;
        }

        float GetLocalUnitY(float position, Unit unit)
        {
            return unit == Unit.Pixels ? position : parent.Size.y * position / 100;
        }
    }
}
