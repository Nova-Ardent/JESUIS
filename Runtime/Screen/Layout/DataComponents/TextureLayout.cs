using UnityEngine;
using JESUIS.Shared.ScreenData.Data;
using UnityEngine.UI;

namespace JESUIS.Runtime.Screen.Layout
{
    public class TextureLayout : BaseLayout
    {
        [SerializeField] protected RawImage rawImage;

        public override void SetLayout(BaseElement baseElement)
        {
            if (baseElement is TextureElement textureElement)
            {
                rawImage.texture = textureElement.Image.Texture.Value;
                rawImage.color = textureElement.Image.Color.Value;
            }
            base.SetLayout(baseElement);
        }

        public override void ReleaseToPool()
        {
            rawImage.texture = null;
            base.ReleaseToPool();
        }
    }
}