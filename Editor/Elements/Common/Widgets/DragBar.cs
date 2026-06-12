using JESUIS.Editor.Elements.Common.Interfaces;
using JESUIS.Editor.Settings;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Common.Widgets
{
    public class DragBar : VisualElement
        , IResizable
    {
        public const string HORIZONTAL_DRAG_BAR_HOVER_ICON_NAME = "d_ResizeHorizontal";

        public const float DRAG_BAR_END_PADDING = 1f;
        public const float DRAG_BAR_EDGE_PADDING = 0f;
        public const float DRAG_BAR_SIZE = 2f;
        public const float DRAG_BAR_TOTAL_SIZE = DRAG_BAR_EDGE_PADDING * 2 + DRAG_BAR_SIZE;

        bool isVerticalSplit = false;
        bool isDragging = false;
        float barDraggedDistance = 0;

        float? min;
        float? max;

        Action onMoved;
        IMGUIContainer cursorLayer;

        public DragBar(bool isVerticalSplit)
        {
            this.isVerticalSplit = isVerticalSplit;
            style.backgroundColor = Colors.DRAG_BAR_COLOR;

            cursorLayer = new IMGUIContainer(DrawCursor);
            cursorLayer.style.position = Position.Absolute;
            cursorLayer.style.left = 0;
            cursorLayer.style.right = 0;
            cursorLayer.style.top = 0;
            cursorLayer.style.bottom = 0;
            Add(cursorLayer);
        }

        public void RegisterCallbacks(Action onBarMoved)
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            parent.RegisterCallback<PointerUpEvent>(OnPointerUp);
            parent.RegisterCallback<PointerMoveEvent>(OnPointerMove);

            onMoved = onBarMoved;
        }

        void DrawCursor()
        {
            var rect = cursorLayer.contentRect;
            if (rect.width <= 0 || rect.height <= 0)
                return;

            EditorGUIUtility.AddCursorRect(new Rect(0, 0, rect.width, rect.height), isVerticalSplit ? MouseCursor.SplitResizeLeftRight : MouseCursor.SplitResizeUpDown);
        }

        public void Resize(float width, float height)
        {
            if (isVerticalSplit)
            {
                style.width = DRAG_BAR_SIZE;
                style.height = height - DRAG_BAR_END_PADDING * 2;
            }
            else
            {
                style.width = width - DRAG_BAR_END_PADDING * 2;
                style.height = DRAG_BAR_SIZE;
            }
        }

        public float GetDragPosition()
        {
            return barDraggedDistance;
        }

        public void SetBounds(float min, float max)
        {
            this.min = min;
            this.max = max;
            barDraggedDistance = Mathf.Clamp(barDraggedDistance, min, max);
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
            }
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging)
                return;

            if (isVerticalSplit)
            {
                barDraggedDistance += evt.deltaPosition.x;
            }
            else
            {
                barDraggedDistance += evt.deltaPosition.y;
            }

            if (min.HasValue && max.HasValue)
            {
                barDraggedDistance = Mathf.Clamp(barDraggedDistance, min.Value, max.Value);
            }

            onMoved();
        }
    }
}