using JESUIS.Shared.ScreenData.DataBindings;
using System;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input.Bindable
{
    public class BindableObjectFieldElement<T> : VisualElement where T : UnityEngine.Object
    {
        protected VisualElement bindingContainer = new VisualElement();
        protected VisualElement inputContainer = new VisualElement();

        ObjectFieldElement<T> objectField; 
        BindingElement<T> bindingElement;
        Action<Bindable<T>> onValueChanged; 

        public BindableObjectFieldElement(string labelText)
        {
            style.flexDirection = FlexDirection.Row;

            bindingElement = new BindingElement<T>();
            bindingElement.style.marginLeft = 11;
            bindingContainer.Add(bindingElement);
            Add(bindingContainer);

            objectField = new ObjectFieldElement<T>(labelText);
            inputContainer.Add(objectField);
            Add(inputContainer);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void Set(Bindable<T> bindable)
        {
            SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);
            SetValue(bindable.Value);
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

        public void SetBinding(ulong high, ulong low)
        {
            bindingElement.SetBinding(high, low);
        }

        public void SetValueWithoutNotify(T value)
        {
            objectField?.SetValueWithoutNotify(value);
        }

        public void SetValue(T value)
        {
            objectField?.SetValue(value);
        }

        public void RegisterOnValueChanged(Action<Bindable<T>> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
                objectField.RegisterOnValueChanged(val =>
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
                        Value = objectField.CurrentValue,
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

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            bindingContainer.style.width = 20;
            inputContainer.style.flexGrow = 1;
        }
    }
}
