using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Common.Widgets
{
    public class CollapseIcon : VisualElement
    {
        public const int SIZE = 12;

        public bool IsCollapsed { get; private set; } = true;
        Image icon;
        Action<bool> onChange;

        public CollapseIcon(Action<bool> onChange) : this()
        {
            RegisterOnCollapseChange(onChange);
        }

        public CollapseIcon()
        {
            style.width = SIZE;
            style.height = SIZE;

            icon = new Image();
            icon.style.width = SIZE;
            icon.style.height = SIZE;
            icon.image = EditorGUIUtility.IconContent("d_icon dropdown").image;
            Add(icon);
            UpdateIcon();
            
            RegisterCallback<PointerDownEvent>(OnMouseDown);
        }

        public void RegisterOnCollapseChange(Action<bool> onChange)
        {
            this.onChange = onChange;
        }

        void UpdateIcon()
        {
            icon.style.rotate = new StyleRotate(new Rotate(IsCollapsed ? -90 : 0));
        }

        public void Toggle()
        {
            IsCollapsed = !IsCollapsed;
            UpdateIcon();
        }

        void OnMouseDown(PointerDownEvent mouseDownEvent)
        {
            if (mouseDownEvent.button == 0)
            {
                Toggle();
                onChange(IsCollapsed);
            }
        }
    }
}
