using JESUIS.Editor.Helpers.Utils;
using JESUIS.Editor.UIBuilder.Panels.Views;
using JESUIS.Shared.ScreenData;
using JESUIS.Shared.ScreenData.Data;
using System;

namespace JESUIS.Editor.UIBuilder.Data
{
    public class EditorState
    {
        Action<EditorViews> selectedElementIsDirty;

        public ReactiveProperty<BaseElement> SelectedElement = new ReactiveProperty<BaseElement>(null);

        public Screen CurrentScreen;

        public void TriggerSelectedElementIsDirty(EditorViews triggeringView)
        {
            selectedElementIsDirty?.Invoke(triggeringView);
        }

        public void ListenToSelectedElementIsDirty(Action<EditorViews> action)
        {
            if (selectedElementIsDirty == null)
            {
                selectedElementIsDirty = action;
            }
            else
            {
                selectedElementIsDirty += action;
            }
        }
    }
}

