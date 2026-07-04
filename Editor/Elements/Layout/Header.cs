using Codice.Client.BaseCommands;
using JESUIS.Editor.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Layout
{
    public class Header : VisualElement
    {
        public const int HEADER_HEIGHT = 20;

        Label mainLabel;// = new Label();
        Label subLabel;// = new Label();
        Image image;// = new Image();

        public Header(string mainText, string subText, Texture icon = null)
        {
            style.height = HEADER_HEIGHT;
            style.width = Length.Percent(100);

            style.borderTopWidth = 1;
            style.borderTopColor = Colors.HEADER_TRIM_TOP;

            style.borderBottomWidth = 1;
            style.borderBottomColor = Colors.HEADER_TRIM_BOTTOM;

            style.backgroundColor = Colors.HEADER_BACKING;

            mainLabel = new Label(mainText);
            mainLabel.style.position = Position.Absolute;
            mainLabel.style.left = 0;
            if (icon == null)
            {
                mainLabel.style.paddingLeft = 10;
            }
            else
            {
                mainLabel.style.paddingLeft = 30;
            }
            mainLabel.style.top = 0;
            mainLabel.style.height = Length.Percent(100);
            mainLabel.style.width = Length.Percent(50);
            mainLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            mainLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            Add(mainLabel);

            subLabel = new Label(subText);
            subLabel.style.position = Position.Absolute;
            subLabel.style.right = 0;
            subLabel.style.paddingRight = 10;
            subLabel.style.top = 0;
            subLabel.style.height = Length.Percent(100);
            subLabel.style.width = Length.Percent(50);
            subLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
            subLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            Add(subLabel);

            if (icon != null)
            {
                image = new Image();
                image.style.position = Position.Absolute;
                image.style.left = 5;
                image.style.top = 1;
                image.style.width = 18;
                image.style.height = 18;
                image.image = icon;
                Add(image);
            }
        }
    }
}
