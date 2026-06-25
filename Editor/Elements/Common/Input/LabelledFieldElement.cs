using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Common.Input
{
    public class LabelledFieldElement : VisualElement
    {
        public const int DEFAULT_TOTAL_PERCENT_WIDTH = 100;
        public const int DEFAULT_LABEL_PERCENT_WIDTH = 40;
        public const int DEFAULT_INPUT_FIELD_PERCENT_WIDTH = 60;
        public const int DEFAULT_LABEL_PADDING_LEFT = 25;

        public const int HEIGHT = 20;

        Label label;
        protected VisualElement FieldContainer {  get; private set; }

        public LabelledFieldElement(string labelText)
        {
            style.width = Length.Percent(DEFAULT_TOTAL_PERCENT_WIDTH);
            style.height = HEIGHT;

            label = new Label(labelText);
            label.style.position = Position.Absolute;
            label.style.left = 0;
            label.style.top = 0;

            label.style.width = Length.Percent(DEFAULT_LABEL_PERCENT_WIDTH);
            label.style.height = HEIGHT;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.paddingLeft = DEFAULT_LABEL_PADDING_LEFT;
            Add(label);

            FieldContainer = new VisualElement();
            FieldContainer.style.position = Position.Absolute;
            FieldContainer.style.left = Length.Percent(DEFAULT_LABEL_PERCENT_WIDTH);
            FieldContainer.style.top = 0;

            FieldContainer.style.width = Length.Percent(DEFAULT_INPUT_FIELD_PERCENT_WIDTH);
            FieldContainer.style.height = HEIGHT;
            Add(FieldContainer);
        }

        public void ChangeLabel(string newLabel)
        {
            label.text = newLabel;
        }
    }
}
