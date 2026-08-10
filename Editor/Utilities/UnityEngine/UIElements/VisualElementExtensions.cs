namespace UnityEngine.UIElements
{
    public static class VisualElementExtensions
    {
        public static void SetPosition(this VisualElement element, Vector2 position)
        {
            element.style.left = position.x;
            element.style.top = position.y;
        }

        public static Vector2 GetRelativeDelta(this VisualElement element, Vector2 delta)
        {
            Vector2 previousLocal = element.WorldToLocal(-delta);
            Vector2 currentLocal = element.WorldToLocal(Vector2.zero);
            return currentLocal - previousLocal;
        }

        public static Vector2 GetRelativeDelta(this VisualElement target, VisualElement source, Vector2 delta)
        {
            Vector2 sourcePreviousLocal = source.LocalToWorld(-delta);
            Vector2 sourceCurrentLocal = source.LocalToWorld(Vector2.zero);

            Vector2 targetPreviousLocal = target.WorldToLocal(sourcePreviousLocal);
            Vector2 targetCurrentLocal = target.WorldToLocal(sourceCurrentLocal);

            return targetCurrentLocal - targetPreviousLocal;
        }
    }
}
