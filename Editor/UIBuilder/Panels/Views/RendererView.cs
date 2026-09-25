using JESUIS.Editor.Elements.Layout.TabBarWidgets;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class RendererView : EditorViews
    {
        public override Views Type => Views.Renderer;

        BoxSelector boxSelector;
        
        RendererDisplay rendererDisplay;
        VisualElement boxSelectedContainer;

        RendererController controller;

        AspectRatioDropDown aspectRatioDropDown;

        public RendererView(EditorState editorState) : base(editorState)
        {
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

            aspectRatioDropDown = new AspectRatioDropDown(OnChangeAspectRatio);

            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryReady);
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
            
            boxSelector.InitializeDragPoints(this);
            boxSelector.OnZoomChanged();
            boxSelector.SetActive(false);
        }

        void OnChangeAspectRatio(int width, int height)
        {
            aspectRatioDropDown.SetAspectRatio(width, height);
            if (width <= 0 || height <= 0)
            {
                return;
            }

            controller.ChangeAspectRatio(width, height);
            if (CurrentEditorState.SelectedElement.Value == CurrentEditorState.CurrentScreen.Value.GetRootElement())
            {
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            }
        }

        protected override void OnSelectedElementChanged(BaseElement selectedElement)
        {
            rendererDisplay.OnSelectedElementChanged(selectedElement);
        }

        protected override void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (triggeringView.Type == Views.Renderer)
            {
                return;
            }

            if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ValueUpdated && elementChanges.TargetElement is RootElement)
            {
                UnityEngine.Vector2 size = elementChanges.TargetElement.Transform.Size.Value;
                if (!Mathf.Approximately(size.x, controller.CurrentWidth) || !Mathf.Approximately(size.y, controller.CurrentHeight))
                {
                    OnChangeAspectRatio(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y));
                }
            }

            rendererDisplay.OnElementIsDirty(elementChanges);
        }

        protected override void OnCurrentScreenChanged(JESUIS.Shared.ScreenData.Screen currentScreen)
        {
            rendererDisplay.OnCurrentScreenChanged(currentScreen);
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
