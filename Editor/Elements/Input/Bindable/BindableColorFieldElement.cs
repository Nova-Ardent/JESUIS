using JESUIS.Shared.ScreenData.DataBindings;
using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.Elements.Input.Bindable
{
    public class BindableColorFieldElement : VisualElement
    {
        protected VisualElement bindingContainer = new VisualElement();
        protected VisualElement inputContainer = new VisualElement();

        ColorFieldElement colorField;
        BindingElement<Color> bindingElement;

        Action<Bindable<Color>> onValueChanged;

        public BindableColorFieldElement(string labelText, Color color)
        {
            style.flexDirection = FlexDirection.Row;

            bindingElement = new BindingElement<Color>();
            bindingElement.style.marginLeft = 11;
            bindingContainer.Add(bindingElement);
            Add(bindingContainer);

            colorField = new ColorFieldElement(labelText, color, LabelledFieldElement.LabelType.MainUnpadded);
            inputContainer.Add(colorField);
            Add(inputContainer);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void Set(Bindable<Color> bindable)
        {
            SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);
            SetValue(bindable.Value);
        }

        public void SetWithoutNotify(Bindable<Color> bindable)
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

        public void SetValueWithoutNotify(Color value)
        {
            colorField?.SetValueWithoutNotify(value);
        }

        public void SetValue(Color value)
        {
            colorField.SetValue(value);
        }

        public void RegisterOnValueChanged(Action<Bindable<Color>> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
                colorField.RegisterOnValueChanged(val =>
                {
                    onValueChanged?.Invoke(new Bindable<Color>
                    {
                        Value = val,
                        UIDHigh = bindingElement.UIDHigh,
                        UIDLow = bindingElement.UIDLow
                    });
                });

                bindingElement.RegisterOnValueChanged(() =>
                {
                    onValueChanged?.Invoke(new Bindable<Color>
                    {
                        Value = colorField.CurrentValue,
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
            bindingContainer.style.width = 40;
            inputContainer.style.width = parent.contentRect.width - 40;
        }
    }
}