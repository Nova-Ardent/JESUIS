namespace UnityEngine
{
    public static class Vector2Extensions
    {
        public static Vector2 RotatePoint(this Vector2 point, Vector2 pivot, float angleDegrees)
        {
            float radians = angleDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            Vector2 dir = point - pivot;

            return new Vector2(
                dir.x * cos - dir.y * sin,
                dir.x * sin + dir.y * cos
            ) + pivot;
        }

        public static float GetAngleDegrees(this Vector2 pivot, Vector2 point)
        {
            Vector2 dir = point - pivot;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }
}