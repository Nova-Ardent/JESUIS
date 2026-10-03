using JESUIS.Shared.ScreenData.DataBindings;
using System;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Input.Bindable
{
    public class BindableInputFieldElement<T> : VisualElement
    {
        protected VisualElement bindingContainer = new VisualElement();
        protected VisualElement inputContainer = new VisualElement();

        protected InputFieldElement<T> inputField;
        protected BindingElement<T> bindingElement;
        Action<Bindable<T>> onValueChanged;

        protected BindableInputFieldElement(InputFieldElement<T> inputField)
        {
            style.flexDirection = FlexDirection.Row;

            bindingElement = new BindingElement<T>();
            bindingElement.style.marginLeft = 11;
            bindingContainer.Add(bindingElement);
            Add(bindingContainer);

            this.inputField = inputField;
            inputContainer.Add(inputField);
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
            inputField?.SetValueWithoutNotify(value);
        }

        public void SetValue(T value)
        {
            inputField?.SetValue(value);
        }

        public void RegisterOnValueChanged(Action<Bindable<T>> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
                inputField.RegisterOnValueChanged(val =>
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
                        Value = inputField.CurrentValue,
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
            inputField.Lock(isLocked);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            bindingContainer.style.width = 40;
            inputContainer.style.width = parent.contentRect.width - 40;
        }
    }

    public abstract class BindableInputFieldElement<BindingType, SubType1, SubType2> : VisualElement
    {
        protected VisualElement bindingContainer = new VisualElement();
        protected VisualElement inputContainer = new VisualElement();

        protected InputFieldElement<SubType1, SubType2> inputField;
        protected BindingElement<BindingType> bindingElement;
        Action<Bindable<BindingType>> onValueChanged;


        protected BindableInputFieldElement(InputFieldElement<SubType1, SubType2> inputField)
        {
            style.flexDirection = FlexDirection.Row;

            bindingElement = new BindingElement<BindingType>();
            bindingElement.style.marginLeft = 11;
            bindingContainer.Add(bindingElement);
            Add(bindingContainer);

            this.inputField = inputField;
            inputContainer.Add(inputField);
            Add(inputContainer);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public abstract SubType1 GetSubType1(BindingType bindingValue);
        public abstract SubType2 GetSubType2(BindingType bindingValue);
        public abstract BindingType BuildBindingType(SubType1 s1, SubType2 s2);

        public void Set(Bindable<BindingType> bindable)
        {
            SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);
            SetValue(GetSubType1(bindable.Value), GetSubType2(bindable.Value));
        }

        public void SetWithoutNotify(Bindable<BindingType> bindable)
        {
            SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);
            SetValueWithoutNotify(GetSubType1(bindable.Value), GetSubType2(bindable.Value));
        }

        public void SetBindingWithoutNotify(ulong high, ulong low)
        {
            bindingElement.SetBindingWithoutNotify(high, low);
        }

        public void SetBinding(ulong high, ulong low)
        {
            bindingElement.SetBinding(high, low);
        }

        public void SetValueWithoutNotify(SubType1 s1, SubType2 s2)
        {
            inputField?.SetValuesWithoutNotify(s1, s2);
        }

        public void SetValue(SubType1 s1, SubType2 s2)
        {
            inputField?.SetValue(s1, s2);
        }

        public void RegisterOnValueChanged(Action<Bindable<BindingType>> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
                inputField.RegisterOnValueChanged((s1, s2) =>
                {
                    Bindable<BindingType> bindable = new Bindable<BindingType>();
                    bindable.Value = BuildBindingType(s1, s2);
                    bindable.UIDHigh = bindingElement.UIDHigh;
                    bindable.UIDLow = bindingElement.UIDLow;
                    onValueChanged?.Invoke(bindable);
                });

                bindingElement.RegisterOnValueChanged(() =>
                {
                    Bindable<BindingType> bindable = new Bindable<BindingType>();
                    bindable.Value = BuildBindingType(inputField.CurrentValue1, inputField.CurrentValue2);
                    bindable.UIDHigh = bindingElement.UIDHigh;
                    bindable.UIDLow = bindingElement.UIDLow;
                    onValueChanged?.Invoke(bindable);
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
            inputField.Lock(isLocked);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            bindingContainer.style.width = 40;
            inputContainer.style.width = parent.contentRect.width - 40;
        }
    }
}