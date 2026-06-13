using JESUIS.Editor.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Common.Layout.TabBarElements
{
    public class DropDown : TabElement
    {
        public const int DROPDOWN_ICON_WIDTH = 18;
        public const int DROPDOWN_ICON_HEIGHT = 12;

        Label label;
        Image dropDownIcon;

        string staticName = null;
        List<NamedAction> options;

        public DropDown(float width, params NamedAction[] options) : this(width, options.AsEnumerable())
        {
        }

        public DropDown(float width, IEnumerable<NamedAction> options) : this(null, width, options)
        {
        }

        public DropDown(string staticName, float width, params NamedAction[] options) : this(staticName, width, options.AsEnumerable())
        {
        }

        public DropDown(string staticName, float width, IEnumerable<NamedAction> options) : base(width)
        {
            this.staticName = staticName;
            style.backgroundColor = Settings.Colors.TAB_DROPDOWN_COLOR;

            this.options = options?.ToList() ?? new List<NamedAction>();
            if (staticName != null)
            {
                InitLabel(staticName);
            }
            else if (options.Count() != 0)
            {
                InitLabel(this.options.First().Name);
            }

            InitDropDownIcon();

            RegisterCallback<PointerEnterEvent>(OnMouseEnter);
            RegisterCallback<PointerLeaveEvent>(OnMouseLeave);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        void InitDropDownIcon()
        {
            dropDownIcon = new Image();
            dropDownIcon.style.position = Position.Absolute;
            dropDownIcon.style.right = 5;
            dropDownIcon.style.top = (style.height.value.value - DROPDOWN_ICON_HEIGHT) / 2;
            dropDownIcon.style.width = DROPDOWN_ICON_WIDTH;
            dropDownIcon.style.height = DROPDOWN_ICON_HEIGHT;
            dropDownIcon.image = EditorGUIUtility.IconContent("d_icon dropdown").image;
            Add(dropDownIcon);
        }

        void InitLabel(string text)
        {
            label = new Label(text);
            label.style.position = Position.Absolute;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.left = 5;
            label.style.top = 0;
            label.style.width = style.width.value.value - 10;
            label.style.height = style.height.value.value;
            Add(label);
        }

        void OnMouseEnter(PointerEnterEvent pointerEnterEvent)
        {
            style.backgroundColor = Settings.Colors.TAB_DROPDOWN_HOVER_COLOR;
        }

        void OnMouseLeave(PointerLeaveEvent pointerLeaveEvent)
        {
            style.backgroundColor = Settings.Colors.TAB_DROPDOWN_COLOR;
        }

        void OnMouseDown(MouseDownEvent mouseDownEvent)
        {
            if (mouseDownEvent.button == 0 && options != null)
            {
                ContextMenuBuilder.BuildMenu(this.worldBound, OnChange, options);
            }
        }

        public void OnChange(int optionSelected)
        {
            if (staticName != null)
            {
                return;
            }

            label.text = options[optionSelected].Name;
        }
    }
}
