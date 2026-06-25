using UnityEngine.UIElements;
using System;

namespace JESUIS.Editor.Elements.Common.Input
{
    public abstract class InputFieldElement<T> : LabelledFieldElement
    {
        TextField inputField;

        T defaultValue;
        public T CurrentValue { get; private set; }
        
        
        Action<T> onValueChanged;

        public InputFieldElement(string labelText, T defaultValue = default(T)) : base(labelText)
        {
            inputField = new TextField();
            
            inputField.style.width = Length.Percent(100);
            inputField.style.height = Length.Percent(100);
            inputField.style.paddingTop = 0;
            inputField.style.paddingBottom = 2;
            inputField.style.paddingRight = 10;
            inputField.isDelayed = true;
            inputField.RegisterValueChangedCallback(OnValueChanged);

            FieldContainer.Add(inputField); 

            CurrentValue = defaultValue;
            this.defaultValue = defaultValue;
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
