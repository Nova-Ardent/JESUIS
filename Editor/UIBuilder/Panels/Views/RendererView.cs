using JESUIS.Editor.Elements.Layout.TabBarWidgets;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class RendererView : EditorViews
    {
        EditorState editorState;

        public override Views Type => Views.Renderer;

        RendererDisplay rendererDisplay;

        AspectRatioDropDown aspectRatioDropDown;

        public RendererView(EditorState editorState)
        {
            this.editorState = editorState;
            rendererDisplay = new RendererDisplay(editorState);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);

            style.backgroundColor = Colors.RENDERER_BACKGROUND_COLOR;

            Add(rendererDisplay);

            aspectRatioDropDown = new AspectRatioDropDown(UpdateAspectRatio);
            aspectRatioDropDown.SetOption(0, true);
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
