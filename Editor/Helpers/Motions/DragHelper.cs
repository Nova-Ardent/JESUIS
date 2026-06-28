using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Helpers.Motions
{
    public class DragHelper
    {
        Action<Vector2> onPositionChanged;
        public Vector2 Offset { get; private set; }

        bool isMouseDown = false;
        int mouseIndex = 0;

        public DragHelper()
        {            
        }

        public void SetTarget(int mouseIndex, VisualElement target)
        {
            this.mouseIndex = mouseIndex;
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        public void OnMouseDown(MouseDownEvent downEvent)
        {
            if (downEvent.button == mouseIndex)
            {
                isMouseDown = true;
            }

            UpdateOffset(downEvent.mouseDelta);
        }

        public void OnMouseUpEvent(MouseUpEvent upEvent)
        {
            if (upEvent.button == mouseIndex)
            {
                isMouseDown = false;
            }
        }

        public void OnMouseMove(MouseMoveEvent moveEvent)
        {
            if (isMouseDown)
            {
                UpdateOffset(moveEvent.mouseDelta);
            }
        }

        public void OnMouseLeave(MouseLeaveEvent leaveEvent)
        {
            if (isMouseDown)
            {
                isMouseDown = false;
            }
        }

        void UpdateOffset(Vector2 delta)
        {
            Offset += delta;
            onPositionChanged?.Invoke(Offset);
        }

        public void RegisterOnPositionChanged(Action<Vector2> action)
        {
            if (onPositionChanged == null)
            {
                onPositionChanged = action;
            }
            else
            {
                onPositionChanged += action;
            }
        }
    }
}
