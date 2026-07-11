using UnityEngine.UIElements;

namespace JESUIS.Editor.Utilities.StyleSheets
{
    public static class VisualElementExtensions
    {
        public static void AddStyle(this VisualElement element, StyleSheet styleSheet, string className)
        {
            element.styleSheets.Add(styleSheet);
            element.AddToClassList(className);
        }
    }
}
