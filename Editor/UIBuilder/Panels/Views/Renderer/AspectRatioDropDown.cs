using JESUIS.Editor.Elements.Layout.TabBarWidgets;
using JESUIS.Editor.Helpers;
using System.Collections.Generic;
using System;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AspectRatioAttribute : Attribute
    {
        public string DisplayName { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public AspectRatioAttribute(string displayName, int width, int height)
        {
            DisplayName = displayName;
            Width = width;
            Height = height;
        }
    }

    public class AspectRatioDropDown : DropDown
    {
        public enum AspectRatioOptions
        {
            [AspectRatio("Full HD (1920x1080)", 1920, 1080)] FullHD,
            [AspectRatio("WXGA (1366x768)", 1366, 768)] WXGA,
            [AspectRatio("QHD (2560x1440)", 2560, 1440)] HD,
            [AspectRatio("4K UHD (3840x2160)", 3840, 2160)] UHD,
            [AspectRatio("Iphone 17", 1206, 2622)] Iphone17,
            [AspectRatio("Iphone 15", 1179, 2556)] Iphone15,
            [AspectRatio("Pixel 10", 1080, 2424)] Pixel10,
        }

        public AspectRatioDropDown(Action<int, int> onSelection) : base(150, GetActions(onSelection))
        {
        }

        static IEnumerable<NamedAction> GetActions(Action<int, int> onSelection)
        {
            foreach(AspectRatioOptions option in Utilities.Utilities.GetEnums<AspectRatioOptions>())
            {
                AspectRatioAttribute attibute = (AspectRatioAttribute)Attribute.GetCustomAttribute(
                    typeof(AspectRatioOptions).GetField(option.ToString()),
                    typeof(AspectRatioAttribute)
                );

                yield return new NamedAction(attibute.DisplayName, () => onSelection(attibute.Width, attibute.Height), true);
            }
        }
    }
}
