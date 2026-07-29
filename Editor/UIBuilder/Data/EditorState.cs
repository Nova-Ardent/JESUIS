using JESUIS.Editor.Helpers.Utils;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Panels.Views;
using JESUIS.Shared.ScreenData;
using JESUIS.Shared.ScreenData.Data;
using System;

namespace JESUIS.Editor.UIBuilder.Data
{
    public class EditorState
    {
        Action<EditorViews, ElementChanges> elementIsDirty;

        public ReactiveProperty<BaseElement> SelectedElement = new ReactiveProperty<BaseElement>(null);

        public ReactiveProperty<Screen> CurrentScreen = new ReactiveProperty<Screen>(null);

        public void TriggerElementIsDirty(EditorViews triggeringView, ElementChanges elementChange)
        {
            elementIsDirty?.Invoke(triggeringView, elementChange);
        }

        public void ListenToElementIsDirty(Action<EditorViews, ElementChanges> action)
        {
            if (elementIsDirty == null)
            {
                elementIsDirty = action;
            }
            else
            {
                elementIsDirty += action;
            }
        }
    }
}

