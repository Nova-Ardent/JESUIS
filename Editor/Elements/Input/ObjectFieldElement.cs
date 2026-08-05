using JESUIS.Editor.Utilities.StyleSheets;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input
{
    public class ObjectFieldElement<T> : LabelledFieldElement where T : UnityEngine.Object
    {
        ObjectField objectField;

        T defaultValue;
        public T CurrentValue { get; private set; }

        Action<T> onValueChanged;

        public ObjectFieldElement(string labelText) : base(labelText)
        {
            objectField = new ObjectField();
            objectField.objectType = typeof(T);
            objectField.AddStyle(InputFieldsUSS.StyleSheetInstance, "object-field");
            objectField.RegisterValueChangedCallback(OnValueChanged);

            FieldContainer.Add(objectField);

            CurrentValue = null;
            this.defaultValue = null;
        }

        public void SetValueWithoutNotify(T newValue)
        {
            objectField.value = newValue;
            CurrentValue = newValue;
        }

        public void SetValue(T newValue)
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

        void OnValueChanged(ChangeEvent<UnityEngine.Object> onChange)
        {
            CurrentValue = (T)onChange.newValue;
            onValueChanged?.Invoke(CurrentValue);
        }
    }
}
