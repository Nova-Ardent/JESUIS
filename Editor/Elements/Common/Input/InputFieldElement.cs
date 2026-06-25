using UnityEngine;
using UnityEngine.UIElements;
using JESUIS.Editor.Settings;
using System;

namespace JESUIS.Editor.Elements.Common.Input
{
    [System.Serializable]
    public abstract class InputFieldElement<T> : VisualElement
    {
        public const int TOTAL_PERCENT_WIDTH = 100;
        public const int LABEL_PERCENT_WIDTH = 40;
        public const int INPUT_FIELD_PERCENT_WIDTH = 60;
        public const int PADDING = 25;

        public const int HEIGHT = 20;

        Label label;
        TextField inputField;

        T defaultValue;
        public T CurrentValue { get; private set; }
        
        
        Action<T> onValueChanged;

        public InputFieldElement(string labelText, T defaultValue = default(T))
        {
            style.width = Length.Percent(TOTAL_PERCENT_WIDTH);
            style.height = HEIGHT;

            label = new Label(labelText);
            label.style.position = Position.Absolute;
            label.style.left = 0;
            label.style.top = 0;

            label.style.width = Length.Percent(LABEL_PERCENT_WIDTH);
            label.style.height = HEIGHT;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.paddingLeft = PADDING;
            Add(label);

            ChangeLabel(labelText);

            inputField = new TextField();
            inputField.style.position = Position.Absolute;
            inputField.style.left = Length.Percent(LABEL_PERCENT_WIDTH);
            inputField.style.top = 0;

            inputField.style.width = Length.Percent(INPUT_FIELD_PERCENT_WIDTH);
            inputField.style.height = HEIGHT - 2;
            inputField.style.paddingRight = 5;
            inputField.isDelayed = true;
            inputField.RegisterValueChangedCallback(OnValueChanged);

            Add(inputField);

            CurrentValue = defaultValue;
            this.defaultValue = defaultValue;
        }

        public void ChangeLabel(string newLabel)
        {
            label.text = newLabel;
        }

        public void SetValue(T newValue)
        {
            CurrentValue = newValue;
            inputField.value = newValue.ToString();
        }

        public void RegisterOnValueChanged(Action<T> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
            }
            else
            {
                onValueChanged += onChange;
            }
        }

        protected virtual bool IsValueValid(string value)
        {
            return true;
        }

        protected abstract T Convert(string value);

        void OnValueChanged(ChangeEvent<string> changeEvent)
        {
            if (string.IsNullOrEmpty(changeEvent.newValue))
            {
                inputField.value = defaultValue.ToString();
                CurrentValue = defaultValue;
                onValueChanged?.Invoke(CurrentValue);
                return;
            }

            if (!IsValueValid(changeEvent.newValue))
            {
                inputField.value = changeEvent.previousValue;
                return;
            }

            CurrentValue = Convert(changeEvent.newValue);
            onValueChanged?.Invoke(CurrentValue);
        }
    }
}
