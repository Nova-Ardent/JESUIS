using JESUIS.Editor.Elements.Layout.TabBarWidgets;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class EditorViews : VisualElement
    {
        protected EditorState CurrentEditorState { get; private set; }

        public virtual Views Type { get => Views.None; }

        public enum Views
        {
            None,
            File,
            Hierarchy,
            Inspector,
            Renderer,
        }

        public EditorViews(EditorState editorState)
        {
            CurrentEditorState = editorState;
            editorState.ListenToElementIsDirty(OnElementIsDirty);
            editorState.SelectedElement.ListenTo(OnSelectedElementChanged);
            editorState.CurrentScreen.ListenTo(OnCurrentScreenChanged);
        }

        protected virtual void OnElementIsDirty(EditorViews editorViews, ElementChanges elementChanges)
        {
        }

        protected virtual void OnSelectedElementChanged(BaseElement selectedElement)
        {
        }

        protected virtual void OnCurrentScreenChanged(Screen currentScreen)
        {
        }

        public virtual IEnumerable<TabElement> GetActiveTabOptions()
        {
            yield break;
        }
    }
}
