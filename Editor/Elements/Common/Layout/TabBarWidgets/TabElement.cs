using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Common.Layout.TabBarWidgets
{
    public class TabElement : VisualElement
    {
        public TabElement(float width)
        {
            style.position = Position.Absolute;
            style.height = TabBar.COMMON_TAB_BAR_HEIGHT;
            style.width = width;

            style.borderRightColor = Settings.Colors.TAB_BAR_TRIM_COLOR;
            style.borderBottomColor = Settings.Colors.TAB_BAR_TRIM_COLOR;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
        }
    }
}
