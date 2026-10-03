using System;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input
{
    public class EnumFieldElement : EnumFieldElement<Enum>
    {
        public EnumFieldElement(string labelText, Enum defaultValue) : base(labelText, defaultValue)
        {
        }
    }

    public class EnumFieldElement<T> : LabelledFieldElement where T : Enum
    {
        EnumField enumField;
        Action<T> onValueChanged;
        public T CurrentValue { get; private set; }

        public EnumFieldElement(string labelText, T defaultValue, LabelType labelType = LabelType.Main) : base(labelText, labelType)
        {
            enumField = new EnumField(defaultValue);

            enumField.style.width = Length.Percent(100);
            enumField.style.height = Length.Percent(100);
            enumField.style.paddingTop = 0;
            enumField.style.paddingBottom = 2;
            enumField.style.paddingRight = 10;
            enumField.RegisterCallback<ChangeEvent<Enum>>(OnValueChanged);

            FieldContainer.Add(enumField);
        }

        public void SetValueWithoutNotify(T value)
        {
            CurrentValue = value;
            enumField.SetValueWithoutNotify(value);
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

        public override void Lock(bool isLocked)
        {
            base.Lock(isLocked);
            enumField.SetEnabled(!isLocked);
        }

        void OnValueChanged(ChangeEvent<Enum> changeEvent)
        {
            CurrentValue = (T)changeEvent.newValue;
            onValueChanged?.Invoke((T)changeEvent.newValue);
        }
    }
}
