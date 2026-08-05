using JESUIS.Editor.Utilities.StyleSheets;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.Elements.Input
{
    public class ColorFieldElement : LabelledFieldElement
    {
        ColorField colorField;

        Color defaultValue;
        public Color CurrentValue { get; private set; }

        Action<Color> onValueChanged;

        public ColorFieldElement(string labelText, Color defaultValue) : base(labelText)
        {
            colorField = new ColorField();
            colorField.AddStyle(InputFieldsUSS.StyleSheetInstance, "color-field");
            colorField.RegisterValueChangedCallback(OnValueChanged);

            FieldContainer.Add(colorField);

            CurrentValue = defaultValue;
            this.defaultValue = defaultValue;
        }

        public void SetValueWithoutNotify(Color newValue)
        {
            colorField.value = newValue;
            CurrentValue = newValue;
        }

        public void SetValue(Color newValue)
        {
            SetValueWithoutNotify(newValue);
            onValueChanged?.Invoke(CurrentValue);
        }

        public void SetToDefaultWithoutNotify()
        {
            SetValueWithoutNotify(defaultValue);
        }

        public void SetToDefault()
        {
            SetValue(defaultValue);
        }

        public void RegisterOnValueChanged(Action<Color> onChange)
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

        void OnValueChanged(ChangeEvent<Color> onChange) 
        {
            CurrentValue = onChange.newValue;
            onValueChanged?.Invoke(CurrentValue);
        }
    }
}
