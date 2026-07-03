using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input
{
    public class LabelledFieldElement : VisualElement
    {
        public const int DEFAULT_TOTAL_PERCENT_WIDTH = 100;
        public const int DEFAULT_LABEL_PERCENT_WIDTH = 40;
        public const int DEFAULT_INPUT_FIELD_PERCENT_WIDTH = 60;
        public const int DEFAULT_LABEL_PADDING_LEFT = 25;

        public const int SUB_ELEMENT_TOTAL_PERCENT_WIDTH = 100;
        public const int SUB_ELEMENT_LABEL_WIDTH = 15;

        public const int HEIGHT = 20;

        Label label;
        protected VisualElement FieldContainer {  get; private set; }

        public LabelledFieldElement(string labelText, bool isSubElement = false)
        {
            style.width = Length.Percent(DEFAULT_TOTAL_PERCENT_WIDTH);
            style.height = HEIGHT;

            label = new Label(labelText);
            label.style.position = Position.Absolute;
            label.style.left = 0;
            label.style.top = 0;

            label.style.height = HEIGHT;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            if (isSubElement)
            {
                label.style.width = SUB_ELEMENT_LABEL_WIDTH;
            }
            else
            {
                label.style.width = Length.Percent(DEFAULT_LABEL_PERCENT_WIDTH);
                label.style.paddingLeft = DEFAULT_LABEL_PADDING_LEFT;
            }

            Add(label);

            FieldContainer = new VisualElement();
            FieldContainer.style.position = Position.Absolute;

            if (isSubElement)
            {
                FieldContainer.style.left = 0;
                FieldContainer.style.top = 0;
                FieldContainer.style.paddingLeft = SUB_ELEMENT_LABEL_WIDTH;
                FieldContainer.style.width = Length.Percent(100);
                FieldContainer.style.height = HEIGHT;
            }
            else
            {
                FieldContainer.style.left = Length.Percent(DEFAULT_LABEL_PERCENT_WIDTH);
                FieldContainer.style.top = 0;

                FieldContainer.style.width = Length.Percent(DEFAULT_INPUT_FIELD_PERCENT_WIDTH);
                FieldContainer.style.height = HEIGHT;
            }
            Add(FieldContainer);
        }

        public void ChangeLabel(string newLabel)
        {
            label.text = newLabel;
        }
    }
}
