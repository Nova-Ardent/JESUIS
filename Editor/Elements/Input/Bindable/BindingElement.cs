using JESUIS.Editor.Helpers;
using JESUIS.Editor.Resources;
using JESUIS.Editor.Settings;
using JESUIS.Shared.ScreenData.DataBindings;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.Elements.Input.Bindable
{
    public class BindingElement<T> : BindingElement
    {
        public BindingElement() : base(typeof(T))
        {
        }
    }

    public class BindingElement : VisualElement
    {
        Type type { get; set; }
        Action onValueChanged;
        bool isLocked = false;

        public ulong UIDHigh { get; private set; }
        public ulong UIDLow { get; private set; }

        public BindingElement(Type type)
        {
            this.type = type;

            style.top = 1;
            style.width = 18;
            style.height = 18;
            style.backgroundImage = ResourceLoader.Instance.Icons.Inspector.Binding.Value;

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<MouseUpEvent>(OnClick);
            OnMouseLeave(null);
        }

        public void RegisterOnValueChanged(Action onChange)
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

        public void SetBindingWithoutNotify(ulong high, ulong low)
        {
            if (isLocked)
                return;

            UIDHigh = high;
            UIDLow = low;

            if (high == 0 && low == 0)
            {
                this.tooltip = "No binding";
                OnMouseLeave(null);
                return;
            }

            Span<byte> bytes = stackalloc byte[16];
            BitConverter.TryWriteBytes(bytes[..8], high);
            BitConverter.TryWriteBytes(bytes[8..], low);

            System.Guid uid = new System.Guid(bytes);

            Shared.ScreenData.DataBindings.DataBinding binding = DataBindingContainer.GetDataBinding(type, uid);
            if (binding == null)
            {
                Debug.LogError("Failed to find databinding for uid" + uid.ToString());
                UIDHigh = 0;
                UIDLow = 0;
            }
            else
            {
                this.tooltip = $"Binding: {binding.Name} ({binding.UID})";
            }

            OnMouseLeave(null);
        }

        public void SetBinding(ulong high, ulong low)
        {
            if (isLocked)
                return;

            SetBindingWithoutNotify(high, low);
            onValueChanged?.Invoke();
        }

        public void Lock(bool isLocked)
        {
            if (isLocked)
            {
                UIDHigh = 0;
                UIDLow = 0;
                this.isLocked = true;
                ShowLocked();
            }
            else
            {
                this.isLocked = false;
            }
        }

        void OnClick(MouseUpEvent evt)
        {
            if (isLocked)
                return;

            Shared.ScreenData.DataBindings.DataBinding[] bindings = DataBindingContainer.GetDataBindingsOfType(type).ToArray();
            if (bindings.Length == 0)
            {
                Debug.Log($"No bindings of type {type} found.");
                return;
            }

            ContextMenuBuilder.BuildMenu(bindings.Select(x =>
            {
                return new NamedAction(x.Name, () => OnOptionClicked(x), true);
            })
            .Prepend(new NamedAction("Clear Binding", () => ClearBinding(), true))
            .ToArray());
        }

        void ClearBinding()
        {
            SetBinding(0, 0);
        }

        void OnOptionClicked(Shared.ScreenData.DataBindings.DataBinding binding)
        {
            if (isLocked)
                return;

            System.Guid uid = binding.UID;

            Span<byte> bytes = stackalloc byte[16];
            uid.TryWriteBytes(bytes);

            ulong high = BitConverter.ToUInt64(bytes[..8]);
            ulong low = BitConverter.ToUInt64(bytes[8..]);

            SetBinding(high, low);
        }

        void OnMouseEnter(MouseEnterEvent evt)
        {
            if (isLocked)
            {
                ShowLocked();
                return;
            }

            Color color = (UIDHigh != 0 && UIDLow != 0) ? Colors.BINDING_COLOR_HIGHLIGHTED_HAS_BINDING : Colors.BINDING_COLOR_HIGHLIGHTED;

            style.unityBackgroundImageTintColor = color;
            style.borderLeftColor = color;
            style.borderTopColor = color;
            style.borderRightColor = color;
            style.borderBottomColor = color;
        }

        void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (isLocked)
            {
                ShowLocked();
                return;
            }

            Color color = (UIDHigh != 0 && UIDLow != 0) ? Colors.BINDING_COLOR_UNHIGHLIGHTED_HAS_BINDING : Colors.BINDING_COLOR_UNHIGHLIGHTED;

            style.unityBackgroundImageTintColor = color;
            style.borderLeftColor = color;
            style.borderTopColor = color;
            style.borderRightColor = color;
            style.borderBottomColor = color;
        }

        void ShowLocked()
        {
            Color color = Colors.BINDING_LOCKED;

            style.unityBackgroundImageTintColor = color;
            style.borderLeftColor = color;
            style.borderTopColor = color;
            style.borderRightColor = color;
            style.borderBottomColor = color;
        }
    }
}
