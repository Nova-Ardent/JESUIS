using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Display
{
    public class RotatedTexture : VisualElement
    {
        public RotatedTexture(Texture2D texture, float defaultRotation = 0, bool isCentered = false)
        {
            style.width = texture.width;
            style.height = texture.height;
            if (isCentered)
            {
                style.translate = new StyleTranslate(new Translate(-texture.width / 2, -texture.height / 2));
            }

            style.backgroundImage = new StyleBackground(texture);
            SetRotation(defaultRotation);
        }

        public void SetRotation(float angle)
        {
            style.rotate = new Rotate(new Angle(angle, AngleUnit.Degree));
        }
    }
}
