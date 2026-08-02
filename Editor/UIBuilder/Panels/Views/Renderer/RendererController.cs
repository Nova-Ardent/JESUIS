using JESUIS.Editor.Helpers.Motions;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer
{
    public class RendererController
    {
        const float MIN_ZOOM = 0.05f;
        const float MAX_ZOOM = 10f;
        const float ZOOM_SPEED = 0.1f;
        const float ASPECT_RATIO_INIT_PADDING = 150;

        public DragHelper DragHelper { get; private set; } = new DragHelper();
        public ZoomHelper ZoomHelper { get; private set; } = new ZoomHelper();

        float currentWidth = 0;
        public float CurrentWidth 
        {
            get
            {
                return currentWidth;
            }
            private set
            {
                foreach (var target in displayTargets)
                    target.style.width = value;
                currentWidth = value;
            }
        }

        float currentHeight = 0;
        public float CurrentHeight
        {
            get
            {
                return currentHeight;
            }
            private set
            {
                foreach (var target in displayTargets)
                    target.style.height = value;
                currentHeight = value;
            }
        }

        VisualElement[] displayTargets;
        VisualElement displayContainer;

        Action<int, int> onAspectRatioChanged = null;
        Action onTransformChanged = null;
        Action onZoomChanged = null;

        public RendererController(VisualElement displayContainer, params VisualElement[] displayTargets)
        {
            if (displayTargets.Length == 0)
            {
                throw new ArgumentException("At least one display target must be provided.");
            }

            this.displayTargets = displayTargets;
            foreach (var target in displayTargets)
            {
                target.style.position = Position.Absolute;
            }
            this.displayContainer = displayContainer;

            DragHelper.SetTarget(2, displayContainer, true);
            ZoomHelper.SetTarget(displayContainer, MIN_ZOOM, MAX_ZOOM, ZOOM_SPEED, ZoomHelper.ZoomMethod.Multiplicative);

            DragHelper.RegisterOnPositionChanged(UpdateTransform);
            ZoomHelper.RegisterOnChange(UpdateZoom);
        }

        public void RegisterOnRatioChanged(Action<int, int> onChanged)
        {
            if (onAspectRatioChanged ==  null)
            {
                onAspectRatioChanged = onChanged;
            }
            else
            {
                onAspectRatioChanged += onChanged;
            }
        }

        public void RegisterTransformChanged(Action onChanged)
        {
            if (onTransformChanged == null)
            {
                onTransformChanged = onChanged;
            }
            else
            {
                onTransformChanged += onChanged;
            }
        }

        public void RegisterZoomChanged(Action onChanged)
        {
            if (onZoomChanged == null)
            {
                onZoomChanged = onChanged;
            }
            else
            {
                onZoomChanged += onChanged;
            }
        }

        public void ChangeAspectRatio(int width, int height)
        {
            CurrentWidth = width;
            CurrentHeight = height;

            // adjust size to fit panel
            float parentWidth = displayContainer.resolvedStyle.width - ASPECT_RATIO_INIT_PADDING;
            float parentHeight = displayContainer.resolvedStyle.height - ASPECT_RATIO_INIT_PADDING;

            float xScale = (parentWidth / width);
            float yScale = (parentHeight / height);

            float zoom = Mathf.Min(xScale, yScale);

            ZoomHelper.UpdateZoom(zoom, new Vector2(displayContainer.style.width.value.value / 2, displayContainer.style.height.value.value / 2));
            DragHelper.SetOffset(Vector2.zero);

            onAspectRatioChanged?.Invoke(width, height);
        }

        public void UpdateTransform()
        {
            UpdateTransform(DragHelper.Offset);
        }

        void UpdateTransform(Vector2 offset)
        {
            Vector2 centerOffset = GetCenterOffset();

            foreach (var target in displayTargets)
            {
                target.style.left = centerOffset.x + DragHelper.Offset.x;
                target.style.top = centerOffset.y + DragHelper.Offset.y;
            }

            onTransformChanged?.Invoke();
        }

        void UpdateZoom(float newZoom, float previousZoom, Vector2 targetMousePosition)
        {
            Vector2 relativeMousePosition = displayTargets.First().WorldToLocal(targetMousePosition);
            Vector2 prevSize = new Vector2(CurrentWidth, CurrentHeight) * previousZoom;
            Vector2 mousePosition = new Vector2(relativeMousePosition.x / CurrentWidth - 0.5f, relativeMousePosition.y / CurrentHeight - 0.5f);

            Vector2 newSize = new Vector2(CurrentWidth, CurrentHeight) * newZoom;
            
            foreach (var target in displayTargets)
                target.style.scale = new Vector2(newZoom, newZoom);

            Vector2 centerOffset = GetCenterOffset();
            DragHelper.SetOffset
                ( new Vector2
                    ( displayTargets[0].style.left.value.value + (prevSize.x - newSize.x) * mousePosition.x - centerOffset.x
                    , displayTargets[0].style.top.value.value + (prevSize.y - newSize.y) * mousePosition.y - centerOffset.y
                    )
                );

            onZoomChanged?.Invoke();
        }

        Vector2 GetCenterOffset()
        {
            return new Vector2
                ( displayContainer.contentRect.width / 2 - displayTargets[0].style.width.value.value / 2
                , displayContainer.contentRect.height / 2 - displayTargets[0].style.height.value.value / 2
                );
        }
    }
}
