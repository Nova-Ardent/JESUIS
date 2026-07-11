using JESUIS.Editor.Settings;
using JESUIS.Editor.Utilities.StyleSheets;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Layout
{
    public class Header : VisualElement
    {
        Label mainLabel;
        Label subLabel;
        Image image;

        public Header(string mainText, string subText, Texture icon = null)
        {
            this.AddStyle(LayoutUSS.StyleSheetInstance, "header");

            style.borderTopColor = Colors.HEADER_TRIM_TOP;
            style.borderBottomColor = Colors.HEADER_TRIM_BOTTOM;
            style.backgroundColor = Colors.HEADER_BACKING;

            mainLabel = new Label(mainText);
            mainLabel.AddStyle(LayoutUSS.StyleSheetInstance, "header-main-label");

            if (icon == null)
            {
                mainLabel.AddToClassList("header-main-label-without-icon");
            }
            else
            {
                mainLabel.AddToClassList("header-main-label-with-icon");
            }
            Add(mainLabel);

            subLabel = new Label(subText);
            subLabel.AddStyle(LayoutUSS.StyleSheetInstance, "header-sub-label");
            Add(subLabel);

            if (icon != null)
            {
                image = new Image();
                image.AddStyle(LayoutUSS.StyleSheetInstance, "header-image");
                image.image = icon;

                Add(image);
            }
        }
    }
}
