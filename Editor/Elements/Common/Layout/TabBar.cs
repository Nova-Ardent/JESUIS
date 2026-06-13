using JESUIS.Editor.Elements.Common.Layout.TabBarElements;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Common.Layout
{
    public class TabBar : VisualElement
    {
        float currentOptionOffset;
        public const float COMMON_TAB_BAR_HEIGHT = 20f;

        public TabBar()
        {
            style.height = COMMON_TAB_BAR_HEIGHT;
            style.width = Length.Percent(100);
            style.backgroundColor = Settings.Colors.TAB_BAR_COLOR;

            style.borderBottomWidth = 1;
            style.borderBottomColor = Settings.Colors.TAB_BAR_TRIM_COLOR;
        }

        public void AddOption(TabElement visualElement)
        {
            visualElement.style.position = Position.Absolute;
            visualElement.style.left = currentOptionOffset;
            visualElement.style.top = 0;
            Add(visualElement);

            currentOptionOffset += visualElement.style.width.value.value;
        }
    }
}