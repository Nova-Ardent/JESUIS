using JESUIS.Editor.Utilities.StyleSheets;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input
{
    public class ButtonElement : Button
    {
        public ButtonElement(string text) : base()
        {
            this.text = text;

            this.AddStyle(InputFieldsUSS.StyleSheetInstance, "button");
        }
    }
}
