using System;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input
{
    public class EnumFieldElement<T> : LabelledFieldElement where T : Enum
    {
        EnumField enumField;
        Action<T> onValueChanged;

        public EnumFieldElement(string labelText, T defaultValue) : base(labelText)
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

        void OnValueChanged(ChangeEvent<Enum> changeEvent)
        {
            onValueChanged?.Invoke((T)changeEvent.newValue);
        }
    }
}
