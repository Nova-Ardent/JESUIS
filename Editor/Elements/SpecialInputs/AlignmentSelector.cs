using JESUIS.Editor.Settings;
using JESUIS.Shared.ScreenData.Types;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using JESUIS.Editor.Elements.Input.Bindable;
using JESUIS.Shared.ScreenData.DataBindings;

namespace JESUIS.Editor.Elements.SpecialInputs
{
    public class AlignmentSelector : VisualElement
    {
        public const int LABEL_HEIGHT = 20;
        public const int PADDING = 5;

        public Label label;
        public AlignmentTool alignmentTool;

        public AlignmentSelector(string name)
        {
            style.width = AlignmentTool.SIZE + PADDING * 2;
            style.height = AlignmentTool.SIZE + PADDING * 2 + LABEL_HEIGHT;

            alignmentTool = new AlignmentTool();
            alignmentTool.style.position = Position.Absolute;
            alignmentTool.style.top = PADDING + LABEL_HEIGHT;
            alignmentTool.style.left = PADDING;
            Add(alignmentTool);

            label = new Label();
            label.text = name;
            label.style.position = Position.Absolute;
            label.style.height = LABEL_HEIGHT;
            label.style.width = Length.Percent(100);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            Add(label);
        }

        public void SetValueWithoutNotify(Alignment alignment)
        {
            alignmentTool.SetValueWithoutNotify(alignment);
        }

        public void RegisterOnValueChanged(Action<Alignment> onChange)
        {
            alignmentTool.RegisterOnValueChanged(onChange);
        }
    }

    public class BindableAlignmentSelector : VisualElement
    {
        public const int LABEL_HEIGHT = 20;
        public const int PADDING = 5;

        public Label label;
        public BindingElement<Alignment> bindingElement;
        public AlignmentTool alignmentTool;

        Action<Bindable<Alignment>> onValueChanged;

        public BindableAlignmentSelector(string name)
        {
            style.width = AlignmentTool.SIZE + PADDING * 2;
            style.height = AlignmentTool.SIZE + PADDING * 2 + LABEL_HEIGHT;

            alignmentTool = new AlignmentTool();
            alignmentTool.style.position = Position.Absolute;
            alignmentTool.style.top = PADDING + LABEL_HEIGHT;
            alignmentTool.style.left = PADDING;
            Add(alignmentTool);

            bindingElement = new BindingElement<Alignment>();
            bindingElement.style.position = Position.Absolute;
            bindingElement.style.top = 0;
            bindingElement.style.left = -9;
            Add(bindingElement);

            label = new Label();
            label.text = name;
            label.style.position = Position.Absolute;
            label.style.left = 9;
            label.style.height = LABEL_HEIGHT;
            label.style.width = AlignmentTool.SIZE - 18;
            Add(label);
        }

        public void SetWithoutNotify(Bindable<Alignment> bindable)
        {
            SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);
            SetValueWithoutNotify(bindable.Value);
        }

        public void Set(Bindable<Alignment> bindable)
        {
            SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);
            SetValue(bindable.Value);
        }

        public void SetBindingWithoutNotify(ulong uidHigh, ulong uidLow)
        {
            bindingElement.SetBindingWithoutNotify(uidHigh, uidLow);
        }

        public void SetValueWithoutNotify(Alignment alignment)
        {
            alignmentTool.SetValueWithoutNotify(alignment);
        }

        public void SetValue(Alignment alignment)
        {
            alignmentTool.SetValue(alignment);
        }

        public void RegisterOnValueChanged(Action<Bindable<Alignment>> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
                alignmentTool.RegisterOnValueChanged(val =>
                {
                    onValueChanged?.Invoke(new Bindable<Alignment>
                    {
                        Value = val,
                        UIDHigh = bindingElement.UIDHigh,
                        UIDLow = bindingElement.UIDLow
                    });
                });

                bindingElement.RegisterOnValueChanged(() =>
                {
                    onValueChanged?.Invoke(new Bindable<Alignment>
                    {
                        Value = alignmentTool.GetValue(),
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
    }

    public class AlignmentTool : VisualElement
    {
        public const int SIZE = 40;

        public class AlignmentPin : VisualElement
        {
            public const int SIZE = 8;

            Alignment alignment;
            bool isSelected = false;

            public AlignmentPin(Alignment alignment)
            {
                style.width = SIZE;
                style.height = SIZE;
                style.backgroundColor = Colors.ALIGNMENT_SELECTOR_PIN_UNSELECTED;

                this.alignment = alignment;

                RegisterCallback<MouseEnterEvent>(OnEnter);
                RegisterCallback<MouseLeaveEvent>(OnExit);
            }

            public Alignment GetAlignment()
            {
                return alignment;
            }

            public void RegisterAlignmentCallback(Action<Alignment, AlignmentPin, bool> onClick)
            {
                RegisterCallback<MouseDownEvent>((evt) =>
                {
                    onClick?.Invoke(alignment, this, true);
                });
            }

            public void SetSelected(bool selected)
            {
                isSelected = selected;
                if (isSelected)
                {
                    style.backgroundColor = Colors.ALIGNMENT_SELECTOR_PIN_SELECTED;
                }
                else
                {
                    style.backgroundColor = Colors.ALIGNMENT_SELECTOR_PIN_UNSELECTED;
                }
            }

            void OnEnter(MouseEnterEvent evt)
            {
                if (isSelected)
                    return;

                style.backgroundColor = Colors.ALIGNMENT_SELECTOR_PIN_HOVER;
            }

            void OnExit(MouseLeaveEvent evt)
            {
                if (isSelected)
                    return;

                style.backgroundColor = Colors.ALIGNMENT_SELECTOR_PIN_UNSELECTED;
            }
        }

        Action<Alignment> onChange = null;
        AlignmentPin activePin = null;
        AlignmentPin[,] alignmentPins = new AlignmentPin[3, 3];

        public AlignmentTool()
        {
            style.backgroundColor = Colors.ALIGNMENT_SELECTOR_BACKING;
            style.width = SIZE;
            style.height = SIZE;

            CreatePins();
        }

        public Alignment GetValue()
        {
            return activePin.GetAlignment();
        }

        public void SetValue(Alignment alignment)
        {
            ValueChanged(alignment, alignmentPins.Cast<AlignmentPin>().First(x => x.GetAlignment() == alignment), true);
        }

        public void SetValueWithoutNotify(Alignment alignment)
        {
            ValueChanged(alignment, alignmentPins.Cast<AlignmentPin>().First(x => x.GetAlignment() == alignment), false);
        }

        public void RegisterOnValueChanged(Action<Alignment> onChange)
        {
            if (this.onChange == null)
            {
                this.onChange = onChange;
            }
            else
            {
                this.onChange += onChange;
            }
        }

        void CreatePins()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    AlignmentPin pin = new AlignmentPin((Alignment)(i + j * 3));
                    pin.RegisterAlignmentCallback(ValueChanged);
                    pin.style.position = Position.Absolute;
                    
                    if (i == 0)
                        pin.style.left = -AlignmentPin.SIZE / 2;
                    else if (i == 1)
                        pin.style.left = (SIZE - AlignmentPin.SIZE) / 2;
                    else
                        pin.style.right = -AlignmentPin.SIZE / 2;

                    if (j == 0)
                        pin.style.top = -AlignmentPin.SIZE / 2;
                    else if (j == 1)
                        pin.style.top = (SIZE - AlignmentPin.SIZE) / 2;
                    else
                        pin.style.bottom = -AlignmentPin.SIZE / 2;

                    alignmentPins[i, j] = pin;
                    Add(pin);
                }
            }

            activePin = alignmentPins[1, 1];
            activePin.SetSelected(true);
        }

        void ValueChanged(Alignment alignment, AlignmentPin selectedPin, bool notify = true)
        {
            if (activePin != null)
            {
                activePin.SetSelected(false);
            }

            activePin = selectedPin;
            activePin.SetSelected(true);
            if (notify)
            {
                onChange(activePin.GetAlignment());
            }
        }
    }   
}
