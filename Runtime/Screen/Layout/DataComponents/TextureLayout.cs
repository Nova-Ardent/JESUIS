using UnityEngine;
using JESUIS.Shared.ScreenData.Data;
using UnityEngine.UI;
using JESUIS.Runtime.Screen.Data;

namespace JESUIS.Runtime.Screen.Layout
{
    public class TextureLayout : BaseLayout
    {
        class ModelTexture
        {
            public ModelDataWrapperListener<Texture2D> Texture;
            public ModelDataWrapperListener<Color> Color;
        }

        [SerializeField] protected RawImage rawImage;
        ModelTexture modelTexture;

        protected override void OnLayoutAndModelSet()
        {
            if (BaseElement is TextureElement textureElement)
            {
                modelTexture = new ModelTexture();
                SetupWrapperListener(ref modelTexture.Texture, textureElement.Image.Texture);
                SetupWrapperListener(ref modelTexture.Color, textureElement.Image.Color);
            }
            else
            {
                throw new System.ArgumentException($"Base Element {BaseElement} is not of type TextureElement");
            }

            base.OnLayoutAndModelSet();
        }

        protected override void PostModelSync()
        {
            if (modelTexture != null)
            {
                rawImage.texture = modelTexture.Texture.Value;
                rawImage.color = modelTexture.Color.Value;
            }
            
            base.PostModelSync();
        }

        public override void ReleaseToPool()
        {
            modelTexture = null;
            rawImage.texture = null;
            base.ReleaseToPool();
        }
    }
}