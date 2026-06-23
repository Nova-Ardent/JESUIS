using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Shared.ScreenData.ScreenDataTypes;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class InspectorView : EditorViews
    {
        EditorState editorState;

        public InspectorView(EditorState editorState)
        {
            this.editorState = editorState;
            editorState.SelectedElement.ListenTo(InspectingNewElement);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
        }

        public void InspectingNewElement(BaseElement baseElement)
        {
            Clear();
            if (baseElement is RootElement)
            {
                return;
            }

            // to do, add serialization here.
        }
    }
}
