using JESUIS.Shared.ScreenData.DataBindings;
using System;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input.Bindable
{
    public class BindableEnumFieldElement<T> : VisualElement where T : Enum
    {
        protected VisualElement bindingContainer = new VisualElement();
        protected VisualElement inputContainer = new VisualElement();

        EnumFieldElement<T> enumField;
        BindingElement bindingElement;

        Action<Bindable<T>> onValueChanged;

        public BindableEnumFieldElement(string labelText, T defaultValue, Type typeOverride = null)
        {
            style.flexDirection = FlexDirection.Row;

            bindingElement = new BindingElement(typeOverride ?? typeof(T));
            bindingElement.style.marginLeft = 11;
            bindingContainer.Add(bindingElement);
            Add(bindingContainer);

            this.enumField = new EnumFieldElement<T>(labelText, defaultValue, LabelledFieldElement.LabelType.MainUnpadded);
            inputContainer.Add(enumField);
            Add(inputContainer);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void SetWithoutNotify(Bindable<T> bindable)
        {
            SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);
            SetValueWithoutNotify(bindable.Value);
        }

        public void SetBindingWithoutNotify(ulong high, ulong low)
        {
            bindingElement.SetBindingWithoutNotify(high, low);
        }

        public void SetValueWithoutNotify(T value)
        {
            enumField?.SetValueWithoutNotify(value);
        }

        public void RegisterOnValueChanged(Action<Bindable<T>> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
                enumField.RegisterOnValueChanged(val =>
                {
                    onValueChanged?.Invoke(new Bindable<T>
                    {
                        Value = val,
                        UIDHigh = bindingElement.UIDHigh,
                        UIDLow = bindingElement.UIDLow
                    });
                });

                bindingElement.RegisterOnValueChanged(() =>
                {
                    onValueChanged?.Invoke(new Bindable<T>
                    {
                        Value = enumField.CurrentValue,
                        UIDHigh = bindingElement.UIDHigh,
                        UIDLow = bindingElement.UIDLow
                    });
                });
            }
            else
            {
                onValueChanged += onChange;
            }
        }

        public void Lock(bool isLocked)
        {
            bindingElement.Lock(isLocked);
            enumField.Lock(isLocked);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            bindingContainer.style.width = 40;
            inputContainer.style.width = parent.contentRect.width - 40;
        }
    }
}
