using JESUIS.Editor.Elements.Layout.TabBarWidgets;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace JESUIS.Editor.Elements.Layout
{
    public class TabBar : VisualElement
    {
        List<TabElement> currentOptions = new List<TabElement>();

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
            if (Children().Contains(visualElement))
            {
                return;
            }

            visualElement.style.position = Position.Absolute;
            visualElement.style.left = currentOptionOffset;
            visualElement.style.top = 0;
            Add(visualElement);

            currentOptionOffset += visualElement.style.width.value.value;
            if (!currentOptions.Contains(visualElement))
            {
                currentOptions.Add(visualElement);
            }
        }

        public void AddOptions(IEnumerable<TabElement> options)
        {
            foreach (var option in options)
            {
                AddOption(option);
            }
        }

        public void RemoveOptions(IEnumerable<TabElement> options)
        {
            foreach (var option in options)
            {
                if (!currentOptions.Contains(option))
                {
                    continue;
                }

                currentOptions.Remove(option);
            }

            Rebuild();
        }

        void Rebuild()
        {
            Clear();
            currentOptionOffset = 0;

            foreach (var option in currentOptions)
            {
                AddOption(option);
            }
        }
    }
}