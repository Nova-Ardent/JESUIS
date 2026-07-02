using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace JESUIS.Editor.Helpers.Motions
{
    public class ZoomHelper
    {
        public enum ZoomMethod
        {
            Additive,
            Multiplicative,
        }

        Action<float, float, Vector2> onChange;

        ZoomMethod zoomMethod;

        float minZoom = 0.1f;
        float maxZoom = 10f;
        float currentZoom = 1f;

        float zoomSpeed = 0.1f;

        public ZoomHelper()
        {

        }

        public void UpdateZoom(float newZoomValue, Vector2 from)
        {
            float wasZoom = currentZoom;
            currentZoom = Mathf.Clamp(newZoomValue, minZoom, maxZoom);
            onChange?.Invoke(currentZoom, wasZoom, from);
        }

        public void RegisterOnChange(Action<float, float, Vector2> onChange)
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

        public void SetTarget(VisualElement target, float minZoom, float maxZoom, float zoomSpeed, ZoomMethod zoomMethod = ZoomMethod.Additive)
        {
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;
            this.zoomSpeed = zoomSpeed;
            this.zoomMethod = zoomMethod;

            target.RegisterCallback<WheelEvent>(OnMouseWheel);
        }

        public void OnMouseWheel(WheelEvent wheelEvent)
        {
            if (!wheelEvent.ctrlKey)
            {
                return;
            }

            float wasZoom = currentZoom;
            if (zoomMethod == ZoomMethod.Additive)
            {
                if (wheelEvent.delta.y < 0)
                {
                    currentZoom = Mathf.Min(currentZoom + zoomSpeed, maxZoom);
                }
                else if (wheelEvent.delta.y > 0)
                {
                    currentZoom = Mathf.Max(currentZoom - zoomSpeed, minZoom);
                }
            }
            else if (zoomMethod == ZoomMethod.Multiplicative)
            {
                if (wheelEvent.delta.y < 0)
                {
                    currentZoom = Mathf.Min(currentZoom * (1 + zoomSpeed), maxZoom);
                }
                else if (wheelEvent.delta.y > 0)
                {
                    currentZoom = Mathf.Max(currentZoom * (1 - zoomSpeed), minZoom);
                }
            }

            onChange?.Invoke(currentZoom, wasZoom, wheelEvent.mousePosition);
        }
    }
}
