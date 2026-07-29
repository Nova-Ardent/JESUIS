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

        AspectRatioDropDown aspectRatioDropDown;

        public RendererView(EditorState editorState)
        {
            this.editorState = editorState;
            editorState.CurrentScreen.ListenTo(OnCurrentScreenChanged);
            editorState.SelectedElement.ListenTo(OnSelectedElementChanged);
            editorState.ListenToElementIsDirty(OnElementIsDirty);

            boxSelector = new BoxSelector(this, editorState);
            rendererDisplay = new RendererDisplay(boxSelector);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);

            style.backgroundColor = Colors.RENDERER_BACKGROUND_COLOR;

            Add(rendererDisplay);
            Add(boxSelector);

            aspectRatioDropDown = new AspectRatioDropDown(UpdateAspectRatio);
            aspectRatioDropDown.SetOption(0, true);
        }

        void OnCurrentScreenChanged(Shared.ScreenData.Screen screen)
        {
            rendererDisplay.SetScreen(screen);
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

        void UpdateAspectRatio(int width, int height) 
        {
            rendererDisplay.ChangeAspectRatio(width, height);
        }
        
        public override IEnumerable<TabElement> GetActiveTabOptions()
        {
            yield return aspectRatioDropDown;
        }
    }
}
