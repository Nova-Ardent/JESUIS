using JESUIS.Editor.Utilities.StyleSheets;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input
{
    public class LabelledFieldElement : VisualElement
    {
        public const int DEFAULT_LABEL_PERCENT_WIDTH = 40;
        public const int DEFAULT_INPUT_FIELD_PERCENT_WIDTH = 60;
        public const int DEFAULT_LABEL_PADDING_LEFT = 25;

        public const int SUB_ELEMENT_LABEL_WIDTH = 15;

        Label label;
        protected VisualElement FieldContainer {  get; private set; }

        public LabelledFieldElement(string labelText, bool isSubElement = false)
        {;
            this.AddStyle(InputFieldsUSS.StyleSheetInstance, "labelled-field-element");

            label = new Label(labelText);
            label.AddStyle(InputFieldsUSS.StyleSheetInstance, "labelled-field-label-common");

            if (isSubElement)
            {
                label.AddToClassList("labelled-field-label-sub");
            }
            else
            {
                label.AddToClassList("labelled-field-label-main");
            }

            Add(label);

            FieldContainer = new VisualElement();
            FieldContainer.AddStyle(InputFieldsUSS.StyleSheetInstance, "field-container-common");

            if (isSubElement)
            {
                FieldContainer.AddToClassList("field-container-sub");
            }
            else
            {
                FieldContainer.AddToClassList("field-container-main");
            }
            Add(FieldContainer);
        }

        public void ChangeLabel(string newLabel)
        {
            label.text = newLabel;
        }
    }
}
