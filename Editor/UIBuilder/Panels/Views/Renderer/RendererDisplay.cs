using JESUIS.Editor.Elements.Common.VisualElements;
using JESUIS.Editor.Resources;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;
using JESUIS.Shared.ScreenData.Data;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer
{
    public class RendererDisplay : MaterialRTTVisualElement
    {
        RendererHierarchyController hierarchyController;

        public RendererDisplay(Shared.ScreenData.Screen currentScreen, BoxSelector boxSelector) : base(ResourceLoader.Instance.Shaders.Background.Value)
        {
            hierarchyController = new RendererHierarchyController(currentScreen, boxSelector);
            Add(hierarchyController);

            OnChangeAspectRatio(100, 100);
        }

        public void OnSelectedElementChanged(BaseElement selectedElement)
        {
            hierarchyController.OnSelectedElementChanged(selectedElement);
        }

        public void OnElementIsDirty(ElementChanges elementChanges)
        {
            hierarchyController.OnElementIsDirty(elementChanges);
        }

        public void OnCurrentScreenChanged(Shared.ScreenData.Screen currentScreen)
        {
            hierarchyController.OnCurrentScreenChanged(currentScreen);
        }

        public void OnChangeAspectRatio(int width, int height)
        {
            SetSize(width, height);
            Material material = GetMaterial();
            material.SetColor("_Color1", Colors.RENDERER_CHECKERBACKGROUND_LIGHT_COLOR);
            material.SetColor("_Color2", Colors.RENDERER_CHECKERBACKGROUND_DARK_COLOR);
            material.SetFloat("_DivisionsHorizontal", width / 10);
            material.SetFloat("_DivisionsVertical", height / 10);
            UpdateTexture();

            hierarchyController.OnChangeAspectRatio(width, height);
        }
    }
}
