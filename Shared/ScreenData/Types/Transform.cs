using UnityEngine;

namespace JESUIS.Shared.ScreenData.Types
{
    [System.Serializable]
    public class Transform
    {
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
    }
}
