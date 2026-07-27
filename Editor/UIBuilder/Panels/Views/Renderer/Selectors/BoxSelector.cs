using JESUIS.Editor.Elements.Display;
using JESUIS.Editor.Resources;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.DragPoints;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.DragPoints.DragPoint;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors
{
    public class BoxSelector : VisualElement
    {
        DragPoint dragPointTopLeft;
        DragPoint dragPointTopMiddle;
        DragPoint dragPointTopRight;

        DragPoint dragPointMiddleLeft;
        DragPoint dragPointMiddleRight;

        DragPoint dragPointBottomLeft;
        DragPoint dragPointBottomMiddle;
        DragPoint dragPointBottomRight;

        RotatedTexture cursorTexture;

        VisualElement target;

        bool isActive = false;

        public BoxSelector(VisualElement parent) 
        {
            parent.Add(this);

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

            InitializeDragPoints();
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

        public VisualElement GetTarget()
        {
            return target;
        }

        void InitializeDragPoints()
        {
            // we're doing a virtual cursor, so we can rotate it.
            Texture2D emptyCursor = ResourceLoader.Instance.Icons.Renderer.EmptyCursor;
            var cursor = new UnityEngine.UIElements.Cursor
            {
                texture = ResourceLoader.Instance.Icons.Renderer.EmptyCursor,
                hotspot = new Vector2(emptyCursor.width / 2, emptyCursor.height / 2),
            };

            cursorTexture = new RotatedTexture(ResourceLoader.Instance.Icons.Renderer.DragArrow.Value, 0, true);
            cursorTexture.style.position = Position.Absolute;
            cursorTexture.style.display = DisplayStyle.None;
            cursorTexture.style.cursor = cursor;
            cursorTexture.pickingMode = PickingMode.Ignore;
            Add(cursorTexture);

            AddDragPoint(ref dragPointTopLeft, 45f, DragEdgeHorizontal.Left, DragEdgeVertical.Top);
            AddDragPoint(ref dragPointTopMiddle, 90f, DragEdgeHorizontal.Middle, DragEdgeVertical.Top);
            AddDragPoint(ref dragPointTopRight, -45f, DragEdgeHorizontal.Right, DragEdgeVertical.Top);

            AddDragPoint(ref dragPointMiddleLeft, 0f, DragEdgeHorizontal.Left, DragEdgeVertical.Middle);
            AddDragPoint(ref dragPointMiddleRight, 0f, DragEdgeHorizontal.Right, DragEdgeVertical.Middle);

            AddDragPoint(ref dragPointBottomLeft, -45f, DragEdgeHorizontal.Left, DragEdgeVertical.Bottom);
            AddDragPoint(ref dragPointBottomMiddle, 90f, DragEdgeHorizontal.Middle, DragEdgeVertical.Bottom);
            AddDragPoint(ref dragPointBottomRight, 45f, DragEdgeHorizontal.Right, DragEdgeVertical.Bottom);

            cursorTexture.BringToFront();
        }

        void AddDragPoint(ref DragPoint dragPoint, float defaultDragAngle, DragEdgeHorizontal horizontal, DragEdgeVertical vertical)
        {
            dragPoint = new DragPoint(this, cursorTexture, defaultDragAngle, horizontal, vertical);
            Add(dragPoint);
            dragPoint.RegisterOnDrag(OnDragPoint);
        }

        void OnTargetGeometryChanged(GeometryChangedEvent geometryChangedEvent)
        {
            WrapToTarget();
        }

        void OnDragPoint(Vector2 localDelta, DragEdgeHorizontal horizontalPoint, DragEdgeVertical verticalPoint)
        {
            if (target == null)
                return;

            if (target is IRendererElement rendererElement)
            {
                Vector2 positionDelta = Vector2.zero;
                
                Shared.ScreenData.Types.Transform transform = rendererElement.GetTransform();
                switch (horizontalPoint)
                {
                    case DragEdgeHorizontal.Left:
                        if (verticalPoint == DragEdgeVertical.Top)
                            positionDelta = target.parent.GetRelativeDelta(target, localDelta);
                        else
                            positionDelta = target.parent.GetRelativeDelta(target, new Vector2(localDelta.x, 0));

                        transform.Size.x -= localDelta.x;
                        break;
                    case DragEdgeHorizontal.Middle: break;
                    case DragEdgeHorizontal.Right:
                        transform.Size.x += localDelta.x;
                        break;
                }

                switch (verticalPoint)
                {
                    case DragEdgeVertical.Top:
                        if (horizontalPoint != DragEdgeHorizontal.Left)
                            positionDelta = target.parent.GetRelativeDelta(target, new Vector2(0, localDelta.y));

                        transform.Size.y -= localDelta.y;
                        break;
                    case DragEdgeVertical.Middle: break;
                    case DragEdgeVertical.Bottom:
                        transform.Size.y += localDelta.y;
                        break;
                }

                transform.Position.x += positionDelta.x;
                transform.Position.y += positionDelta.y;

                rendererElement.OnValuesChanged();
                WrapToTarget();
            }
        }
    }
}
