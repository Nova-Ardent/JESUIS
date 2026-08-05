using JESUIS.Editor.Elements.Display;
using JESUIS.Editor.Resources;
using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.DragPoints
{
    public class DragPoint : VisualElement
    {
        public enum DragEdgeHorizontal
        {
            Left,
            Middle,
            Right,
        }

        public enum DragEdgeVertical
        {
            Top,
            Middle,
            Bottom,
        }

        public const int WIDTH = 10;
        public const int HEIGHT = 10;

        float defaultDragAngle = 0;

        bool isHovering = false;
        bool isDragging = false;

        DragEdgeHorizontal horizontalEdge;
        DragEdgeVertical verticalEdge;

        Action<Vector2, DragEdgeHorizontal, DragEdgeVertical> onDrag;

        BoxSelector parentSelector;
        RotatedTexture cursorTexture;

        public DragPoint(BoxSelector parentSelector, RotatedTexture cursorTexture, float defaultDragAngle, DragEdgeHorizontal horizontal, DragEdgeVertical vertical)
        {
            this.cursorTexture = cursorTexture;
            this.parentSelector = parentSelector;
            this.defaultDragAngle = defaultDragAngle;

            style.position = Position.Absolute;
            style.width = WIDTH;
            style.height = HEIGHT;
            style.translate = new StyleTranslate(new Translate(-WIDTH / 2, -HEIGHT / 2));
            style.backgroundImage = ResourceLoader.Instance.Icons.Renderer.DragPoint.Value;
            style.cursor = cursorTexture.style.cursor;

            horizontalEdge = horizontal;
            switch (horizontal)
            {
                case DragEdgeHorizontal.Left:
                    style.left = 0;
                    break;
                case DragEdgeHorizontal.Middle:
                    style.left = Length.Percent(50);
                    break;
                case DragEdgeHorizontal.Right:
                    style.left = Length.Percent(100);
                    break;
            }

            verticalEdge = vertical;
            switch (vertical)
            {
                case DragEdgeVertical.Top:
                    style.top = 0;
                    break;
                case DragEdgeVertical.Middle:
                    style.top = Length.Percent(50);
                    break;
                case DragEdgeVertical.Bottom:
                    style.top = Length.Percent(100);
                    break;
            }
        }

        public void RegisterOnDrag(VisualElement mouseContainer, Action<Vector2, DragEdgeHorizontal, DragEdgeVertical> dragHandler)
        {
            if (onDrag == null) 
            {
                onDrag = dragHandler;

                RegisterCallback<PointerDownEvent>(OnPointerDown);
                RegisterCallback<PointerEnterEvent>(OnPointerEnter);    
                RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
                RegisterCallback<PointerMoveEvent>(OnPointerMoveOnDragPoint);

                mouseContainer.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                mouseContainer.RegisterCallback<PointerUpEvent>(OnPointerUp);
            }
            else
            {
                onDrag += dragHandler;
            }
        }

        void UpdateCursorState()
        {
            if (isHovering || isDragging)
            {
                cursorTexture.style.display = DisplayStyle.Flex;
            }
            else
            {
                cursorTexture.style.display = DisplayStyle.None;
            }
        }

        void UpdateCursorPosition(Vector2 mousePosition)
        {
            if (isDragging || isHovering)
            {
                Vector2 localMousePosition = this.parent.WorldToLocal(mousePosition);
                cursorTexture.style.left = localMousePosition.x;
                cursorTexture.style.top = localMousePosition.y;
                cursorTexture.SetRotation(defaultDragAngle);
            }
        }

        void OnPointerEnter(PointerEnterEvent evt)
        {
            isHovering = true;
            UpdateCursorState();
            UpdateCursorPosition(evt.position);
        }

        void OnPointerLeave(PointerLeaveEvent evt)
        {
            isHovering = false;
            UpdateCursorState();
            UpdateCursorPosition(evt.position);
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (!isDragging)
            {
                isDragging = true;
                evt.StopImmediatePropagation();
            }
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (isDragging)
            {
                isDragging = false;
                evt.StopImmediatePropagation();
                UpdateCursorState();
                UpdateCursorPosition(evt.position);
            }
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            UpdateCursorPosition(evt.position);

            if (!isDragging)
                return;

            if (parentSelector.GetTarget() == null)
                return;

            onDrag?.Invoke(parentSelector.GetTarget().GetRelativeDelta(evt.deltaPosition), horizontalEdge, verticalEdge);
        }

        void OnPointerMoveOnDragPoint(PointerMoveEvent evt)
        {
            UpdateCursorState();
        }
    }
}
