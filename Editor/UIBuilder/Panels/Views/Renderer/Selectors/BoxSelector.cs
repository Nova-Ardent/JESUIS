using JESUIS.Editor.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors
{
    public class BoxSelector : VisualElement
    {
        VisualElement target;

        bool isActive = false;

        public BoxSelector() 
        {
            style.position = Position.Absolute;
            style.width = 100;
            style.height = 100;
            style.display = DisplayStyle.None;

            style.borderTopWidth = 2;
            style.borderBottomWidth = 2;
            style.borderLeftWidth = 2;
            style.borderRightWidth = 2;

            style.borderTopColor = Colors.RENDERER_BOX_SELECTOR_COLOR;
            style.borderBottomColor = Colors.RENDERER_BOX_SELECTOR_COLOR;
            style.borderLeftColor = Colors.RENDERER_BOX_SELECTOR_COLOR;
            style.borderRightColor = Colors.RENDERER_BOX_SELECTOR_COLOR;

            schedule.Execute(()=>
            {
                if (target != null && GetActive())
                    WrapToTarget();
            }).Every(0);
        }

        public void WrapToTarget()
        {
            style.transformOrigin = new TransformOrigin(0, 0, 0);

            Vector2 topLeft = parent.WorldToLocal(target.LocalToWorld(Vector2.zero));
            Vector2 topRight = parent.WorldToLocal(target.LocalToWorld(new Vector2(target.style.width.value.value, 0)));
            Vector2 bottomLeft = parent.WorldToLocal(target.LocalToWorld(new Vector2(0, target.style.height.value.value)));
            Vector2 bottomRight = parent.WorldToLocal(target.LocalToWorld(new Vector2(target.style.width.value.value, target.style.height.value.value)));

            style.left = topLeft.x;
            style.top = topLeft.y;

            style.width = Vector2.Distance(topLeft, topRight);
            style.height = Vector2.Distance(topLeft, bottomLeft);

            Vector2 angularDiff = topRight - topLeft;
            style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Atan2(angularDiff.y, angularDiff.x), AngleUnit.Radian)));
        }

        public void SetTarget(VisualElement targetElement)
        {
            if (target != null)
            {
                target.UnregisterCallback<GeometryChangedEvent>(OnTargetGeometryChanged);
            }

            target = targetElement;
            if (target == null)
            {
                return;
            }

            target.RegisterCallback<GeometryChangedEvent>(OnTargetGeometryChanged);
            WrapToTarget();
        }

        void OnTargetGeometryChanged(GeometryChangedEvent geometryChangedEvent)
        {
            WrapToTarget();
        }

        public bool GetActive()
        {
            return isActive;
        }

        public void SetActive(bool active)
        {
            if (!active)
                SetTarget(null);

            isActive = active;
            style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
