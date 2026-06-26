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

        public InputFieldElement(string labelText, T defaultValue = default(T), bool isSubElement = false) : base(labelText, isSubElement)
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

        public void SetValueWithoutNotify(T newValue)
        {
            inputField.value = newValue.ToString();
            CurrentValue = newValue;
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

    public abstract class InputFieldElement<I, J> : LabelledFieldElement
    {
        protected InputFieldElement<I> input1;
        protected InputFieldElement<J> input2;

        public InputFieldElement(string labelText, string subLabel1, string subLabel2) : this(labelText, subLabel1, default(I), subLabel2, default(J))
        {
        }

        public InputFieldElement(string labelText, string subLabel1, I default1, string subLabel2, J default2) : base(labelText)
        {
            input1 = CreateInputOne(subLabel1, default1);
            input1.style.position = Position.Absolute;
            input1.style.width = Length.Percent(50);
            input1.style.height = Length.Percent(100);
            input1.style.left = 0;
            input1.style.top = 0;
            input1.RegisterOnValueChanged(OnValueOneChanged);
            FieldContainer.Add(input1);

            input2 = CreateInputTwo(subLabel2, default2);
            input2.style.position = Position.Absolute;
            input2.style.width = Length.Percent(50);
            input2.style.height = Length.Percent(100);
            input2.style.left = Length.Percent(50);
            input2.style.top = 0;
            input2.RegisterOnValueChanged(OnValueTwoChanged);
            FieldContainer.Add(input2);
        }

        public void SetValuesWithoutNotify(I value1, J value2)
        {
            input1.SetValueWithoutNotify(value1);
            input2.SetValueWithoutNotify(value2);
        }

        void OnValueOneChanged(I newValue)
        {
            OnChange(newValue, input2.CurrentValue);
        }

        void OnValueTwoChanged(J newValue)
        {
            OnChange(input1.CurrentValue, newValue);
        }

        protected abstract void OnChange(I i, J j);

        protected abstract InputFieldElement<I> CreateInputOne(string subLabel, I defaultValue);
        protected abstract InputFieldElement<J> CreateInputTwo(string subLabel, J defaultValue);
    }
}
