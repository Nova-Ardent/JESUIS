using JESUIS.Editor.Helpers.Utils;
using JESUIS.Shared.ScreenData;
using JESUIS.Shared.ScreenData.ScreenDataTypes;

namespace JESUIS.Editor.UIBuilder.Data
{
    public class EditorState
    {
        public ReactiveProperty<BaseElement> SelectedElement = new ReactiveProperty<BaseElement>(null);
        public Screen CurrentScreen;
    }
}

