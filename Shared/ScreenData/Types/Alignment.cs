namespace JESUIS.Shared.ScreenData.Types
{
    public enum Alignment
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Middle,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public static class AlignmentExtension
    {
        public static bool IsRight(this Alignment alignment)
        {
            return alignment == Alignment.TopRight
                || alignment == Alignment.Right
                || alignment == Alignment.BottomRight;
        }

        public static bool IsMiddleCol(this Alignment alignment)
        {
            return alignment == Alignment.Top
                || alignment == Alignment.Middle
                || alignment == Alignment.Bottom;
        }

        public static bool IsLeft(this Alignment alignment)
        {
            return alignment == Alignment.TopLeft
                || alignment == Alignment.Left
                || alignment == Alignment.BottomLeft;
        }

        public static bool IsTop(this Alignment alignment)
        {
            return alignment == Alignment.TopLeft
                || alignment == Alignment.Top
                || alignment == Alignment.TopRight;
        }

        public static bool IsMiddleRow(this Alignment alignment)
        {
            return alignment == Alignment.Left
                || alignment == Alignment.Middle
                || alignment == Alignment.Right;
        }

        public static bool IsBottom(this Alignment alignment)
        {
            return alignment == Alignment.BottomLeft
                || alignment == Alignment.Bottom
                || alignment == Alignment.BottomRight;
        }
    }
}

