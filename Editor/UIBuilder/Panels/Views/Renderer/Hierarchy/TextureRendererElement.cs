using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Shared.ScreenData.Data;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy
{
    [RendererElement(typeof(TextureElement))]
    public class TextureRendererElement : EmptyRendererElement, IRendererElement<TextureElement>
    {
        TextureElement IRendererElement<TextureElement>.Data 
        {
            get
            {
                return (TextureElement)base.Data;
            }
            set
            {
                base.Data = value;
            }
        }

        public override void OnValuesChanged()
        {
            base.OnValuesChanged();

            TextureElement data = (TextureElement)Data;

            style.backgroundImage = data.Image.Texture;
            style.unityBackgroundImageTintColor = data.Image.Color;
        }
    }
}