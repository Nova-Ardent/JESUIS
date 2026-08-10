using JESUIS.Editor.Elements.Display;
using JESUIS.Editor.Resources;
using JESUIS.Editor.Settings;
using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.DragPoints
{
    public class RotationPoint : VisualElement
    {
        public const int WIDTH = 12;
        public const int LENGTH = 50;

        VisualElement bar;
        VisualElement head;
        VisualElement tail;

        Action<float> onRotate;

        BoxSelector parentSelector;
        RotatedTexture cursorTexture;

        bool isHovering = false;
        bool isDragging = false;
        Vector2 initialRelativePoint = Vector2.zero;

        float currentCursorAngle = 0;

        public RotationPoint(BoxSelector parentSelector, RotatedTexture cursorTexture)
        {
            this.parentSelector = parentSelector;
            this.cursorTexture = cursorTexture;

            style.width = WIDTH;
            style.height = LENGTH;

            bar = new VisualElement();
            bar.style.position = Position.Absolute;
            bar.style.width = 2;
            bar.style.height = LENGTH - WIDTH;
            bar.style.left = 4;
            bar.style.top = 5;
            bar.style.backgroundColor = Colors.RENDERER_BOX_SELECTOR_COLOR;
            Add(bar);

            head = new VisualElement();
            head.style.backgroundImage = ResourceLoader.Instance.Icons.Renderer.RotatePoint.Value;
            head.style.position = Position.Absolute;
            head.style.width = WIDTH;
            head.style.height = WIDTH;
            head.style.left = 0;
            head.style.top = 0;
            head.style.cursor = cursorTexture.style.cursor;
            Add(head);

            tail = new VisualElement();
            tail.style.backgroundImage = ResourceLoader.Instance.Icons.Renderer.RotatePoint.Value;
            tail.style.position = Position.Absolute;
            tail.style.width = WIDTH;
            tail.style.height = WIDTH;
            tail.style.left = 0;
            tail.style.top = LENGTH - WIDTH;
            tail.style.cursor = cursorTexture.style.cursor;
            Add(tail);

            pickingMode = PickingMode.Ignore;
            bar.pickingMode = PickingMode.Ignore;
            tail.pickingMode = PickingMode.Ignore;

            style.translate = new StyleTranslate(new Translate(-WIDTH / 2, -LENGTH + WIDTH / 2));
            style.transformOrigin = new TransformOrigin(WIDTH / 2, LENGTH - WIDTH / 2, 0);
        }

        public void RegisterOnRotate(VisualElement mouseContainer, Action<float> rotateHandler)
        {
            if (onRotate == null)
            {
                onRotate = rotateHandler;

                head.RegisterCallback<PointerDownEvent>(OnPointerDown);
                head.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
                head.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
                head.RegisterCallback<PointerMoveEvent>(OnPointerMoveOnDragPoint);

                mouseContainer.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                mouseContainer.RegisterCallback<PointerUpEvent>(OnPointerUp);
            }
            else
            {
                onRotate += rotateHandler;
            }
        }

        public void SetCursorAngle(float angle)
        {
            currentCursorAngle = angle;
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
                cursorTexture.SetRotation(currentCursorAngle);
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
                initialRelativePoint = tail.worldBound.center;
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
            
            float angle = (initialRelativePoint.GetAngleDegrees(evt.position) + 450) % 360;
            onRotate?.Invoke(angle);
        }

        void OnPointerMoveOnDragPoint(PointerMoveEvent evt)
        {
            UpdateCursorState();
        }
    }
}
