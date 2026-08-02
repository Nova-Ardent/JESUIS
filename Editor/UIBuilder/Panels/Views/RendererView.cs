using JESUIS.Editor.Elements.Layout.TabBarWidgets;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;
using JESUIS.Shared.ScreenData.Data;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class RendererView : EditorViews
    {
        EditorState editorState;

        public override Views Type => Views.Renderer;

        BoxSelector boxSelector;
        
        RendererDisplay rendererDisplay;
        VisualElement boxSelectedContainer;

        RendererController controller;

        AspectRatioDropDown aspectRatioDropDown;

        public RendererView(EditorState editorState)
        {
            this.editorState = editorState;
            editorState.SelectedElement.ListenTo(OnSelectedElementChanged);
            editorState.ListenToElementIsDirty(OnElementIsDirty);

            boxSelectedContainer = new VisualElement();
            boxSelector = new BoxSelector(this, boxSelectedContainer, editorState);
            rendererDisplay = new RendererDisplay(editorState.CurrentScreen, boxSelector);

            controller = new RendererController(this, rendererDisplay, boxSelectedContainer);
            controller.RegisterOnRatioChanged(rendererDisplay.OnChangeAspectRatio);
            controller.RegisterZoomChanged(boxSelector.OnZoomChanged);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);

            style.backgroundColor = Colors.RENDERER_BACKGROUND_COLOR;

            Add(rendererDisplay);
            Add(boxSelectedContainer);
            boxSelectedContainer.Add(boxSelector);

            aspectRatioDropDown = new AspectRatioDropDown(controller.ChangeAspectRatio);

            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryReady);
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
            boxSelector.InitializeDragPoints(this);
            boxSelector.OnZoomChanged();
        }

        void OnSelectedElementChanged(BaseElement selectedElement)
        {
            rendererDisplay.OnSelectedElementChanged(selectedElement);
        }

        void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (triggeringView.Type == Views.Renderer)
            {
                return;
            }

            rendererDisplay.OnElementIsDirty(elementChanges);
        }
        
        public override IEnumerable<TabElement> GetActiveTabOptions()
        {
            yield return aspectRatioDropDown;
        }

        void OnGeometryReady(GeometryChangedEvent evt)
        {
            aspectRatioDropDown.SetOption(0, true);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            controller.UpdateTransform();
        }
    }
}
