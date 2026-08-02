using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Shared.ScreenData.Data;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy
{
    [RendererElement(typeof(EmptyElement))]
    public class EmptyRendererElement : BaseRendererElement, IRendererElement<EmptyElement>
    {
        EmptyElement IRendererElement<EmptyElement>.Data 
        {
            get
            {
                return (EmptyElement)base.Data;
            }
            set
            {
                base.Data = value;
            }
        }
    }
}
