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
    }
}