using JESUIS.Editor.Elements.Display;
using JESUIS.Editor.Resources;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.DragPoints;
using JESUIS.Shared.ScreenData.Types;
using UnityEngine;
using UnityEngine.UIElements;
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

        RotationPoint rotationPoint;

        RotatedTexture dragCursorTexture;
        RotatedTexture rotationCursorTexture;

        EditorState editorState;
        RendererView rendererView;

        VisualElement container;

        VisualElement target;

        bool isActive = false;

        public BoxSelector(RendererView rendererView, VisualElement container,  EditorState editorState) 
        {
            this.editorState = editorState;
            this.rendererView = rendererView;
            this.container = container;

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
            if (target is IRendererElement rendererElement)
            {
                Shared.ScreenData.Types.Transform transform = rendererElement.GetTransform();
                
                if (transform.Pivot.IsLeft())
                    rotationPoint.style.left = 0;
                else if (transform.Pivot.IsMiddleCol())
                    rotationPoint.style.left = Length.Percent(50);
                else if (transform.Pivot.IsRight())
                    rotationPoint.style.left = Length.Percent(100);

                if (transform.Pivot.IsTop())
                    rotationPoint.style.top = 0;
                else if (transform.Pivot.IsMiddleRow())
                    rotationPoint.style.top = Length.Percent(50);
                else if (transform.Pivot.IsBottom())
                    rotationPoint.style.top = Length.Percent(100);

                style.transformOrigin = new TransformOrigin(0, 0, 0);

                float left = target.style.left.value.value;
                float top = target.style.top.value.value;

                float width = target.style.width.value.value * target.style.scale.value.value.x;
                float height = target.style.height.value.value * target.style.scale.value.value.y;

                Vector2 transformOrigin = new Vector2(left + target.style.transformOrigin.value.x.value, top + target.style.transformOrigin.value.y.value);

                Vector2 localTopLeft = new Vector2(left, top).RotatePoint(transformOrigin, rendererElement.GetTransform().Rotation);
                Vector2 localTopRight = new Vector2(left + width, top).RotatePoint(transformOrigin, rendererElement.GetTransform().Rotation);
                Vector2 localBottomLeft = new Vector2(left, top + height).RotatePoint(transformOrigin, rendererElement.GetTransform().Rotation);
                Vector2 localBottomRight = new Vector2(left + width, top + height).RotatePoint(transformOrigin, rendererElement.GetTransform().Rotation);

                Vector2 topLeft = container.WorldToLocal(target.parent.LocalToWorld(localTopLeft));
                Vector2 topRight = container.WorldToLocal(target.parent.LocalToWorld(localTopRight));
                Vector2 bottomLeft = container.WorldToLocal(target.parent.LocalToWorld(localBottomLeft));
                Vector2 bottomRight = container.WorldToLocal(target.parent.LocalToWorld(localBottomRight));

                style.left = topLeft.x;
                style.top = topLeft.y;

                style.width = Vector2.Distance(topLeft, topRight) / style.scale.value.value.x;
                style.height = Vector2.Distance(topLeft, bottomLeft) / style.scale.value.value.y;

                Vector2 angularDiff = topRight - topLeft;
                style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Atan2(angularDiff.y, angularDiff.x), AngleUnit.Radian)));
            }
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

        public void OnZoomChanged()
        {
            Vector2 scale = container.style.scale.value.value;
            Vector2 inverseScale = new Vector2(1 / scale.x, 1 / scale.y);
            this.style.scale = new StyleScale(inverseScale);

            if (target == null)
                return;

            WrapToTarget();
        }

        public VisualElement GetTarget()
        {
            return target;
        }

        public void InitializeDragPoints(VisualElement mouseContainer)
        {
            // we're doing a virtual cursor, so we can rotate it.
            Texture2D emptyCursor = ResourceLoader.Instance.Icons.Renderer.EmptyCursor;
            var cursor = new UnityEngine.UIElements.Cursor
            {
                texture = ResourceLoader.Instance.Icons.Renderer.EmptyCursor,
                hotspot = new Vector2(emptyCursor.width / 2, emptyCursor.height / 2),
            };

            dragCursorTexture = new RotatedTexture(ResourceLoader.Instance.Icons.Renderer.DragArrow.Value, 0, true);
            dragCursorTexture.style.position = Position.Absolute;
            dragCursorTexture.style.display = DisplayStyle.None;
            dragCursorTexture.style.cursor = cursor;
            dragCursorTexture.pickingMode = PickingMode.Ignore;
            Add(dragCursorTexture);

            AddDragPoint(ref dragPointTopLeft, 45f, DragEdgeHorizontal.Left, DragEdgeVertical.Top, mouseContainer);
            AddDragPoint(ref dragPointTopMiddle, 90f, DragEdgeHorizontal.Middle, DragEdgeVertical.Top, mouseContainer);
            AddDragPoint(ref dragPointTopRight, -45f, DragEdgeHorizontal.Right, DragEdgeVertical.Top, mouseContainer);

            AddDragPoint(ref dragPointMiddleLeft, 0f, DragEdgeHorizontal.Left, DragEdgeVertical.Middle, mouseContainer);
            AddDragPoint(ref dragPointMiddleRight, 0f, DragEdgeHorizontal.Right, DragEdgeVertical.Middle, mouseContainer);

            AddDragPoint(ref dragPointBottomLeft, -45f, DragEdgeHorizontal.Left, DragEdgeVertical.Bottom, mouseContainer);
            AddDragPoint(ref dragPointBottomMiddle, 90f, DragEdgeHorizontal.Middle, DragEdgeVertical.Bottom, mouseContainer);
            AddDragPoint(ref dragPointBottomRight, 45f, DragEdgeHorizontal.Right, DragEdgeVertical.Bottom, mouseContainer);

            rotationCursorTexture = new RotatedTexture(ResourceLoader.Instance.Icons.Renderer.RotateArrow.Value, 0, true);
            rotationCursorTexture.style.position = Position.Absolute;
            rotationCursorTexture.style.display = DisplayStyle.None;
            rotationCursorTexture.style.cursor = cursor;
            rotationCursorTexture.pickingMode = PickingMode.Ignore;
            Add(rotationCursorTexture);

            rotationPoint = new RotationPoint(this, rotationCursorTexture);
            rotationPoint.style.position = Position.Absolute;
            rotationPoint.style.left = 0;
            rotationPoint.style.top = 0;
            rotationPoint.RegisterOnRotate(mouseContainer, OnRotatePoint);
            Add(rotationPoint);

            dragCursorTexture.BringToFront();
            rotationCursorTexture.BringToFront();
        }

        void AddDragPoint(ref DragPoint dragPoint, float defaultDragAngle, DragEdgeHorizontal horizontal, DragEdgeVertical vertical, VisualElement mouseContainer)
        {
            dragPoint = new DragPoint(this, dragCursorTexture, defaultDragAngle, horizontal, vertical);
            Add(dragPoint);
            dragPoint.RegisterOnDrag(mouseContainer, OnDragPoint);
        }

        void OnTargetGeometryChanged(GeometryChangedEvent geometryChangedEvent)
        {
            WrapToTarget();
        }

        void OnRotatePoint(float angle)
        {
            if (target == null)
                return;
            
            if (target is IRendererElement rendererElement)
            {
                Shared.ScreenData.Types.Transform transform = rendererElement.GetTransform();
                transform.Rotation = angle;

                rendererElement.OnValuesChanged();
                editorState.TriggerElementIsDirty(rendererView, new ValuesUpdated(editorState.SelectedElement));
                WrapToTarget();
            }
        }

        void OnDragPoint(Vector2 localDelta, DragEdgeHorizontal horizontalPoint, DragEdgeVertical verticalPoint)
        {
            if (target == null)
                return;

            if (target is IRendererElement rendererElement)
            {
                Vector2 positionDelta = Vector2.zero;
                Vector2 sizeDelta = Vector2.zero;

                Shared.ScreenData.Types.Transform transform = rendererElement.GetTransform();

                switch (horizontalPoint)
                {
                    case DragEdgeHorizontal.Left:
                        if (transform.Pivot.IsLeft())
                            positionDelta = target.parent.GetRelativeDelta(target, new Vector2(localDelta.x, 0));
                        else if (transform.Pivot.IsMiddleCol())
                            positionDelta = target.parent.GetRelativeDelta(target, new Vector2(localDelta.x / 2, 0));

                        sizeDelta -= new Vector2(localDelta.x, 0);
                        break;
                    case DragEdgeHorizontal.Middle: break;
                    case DragEdgeHorizontal.Right:
                        if (transform.Pivot.IsRight())
                            positionDelta = target.parent.GetRelativeDelta(target, new Vector2(localDelta.x, 0));
                        else if (transform.Pivot.IsMiddleCol())
                            positionDelta = target.parent.GetRelativeDelta(target, new Vector2(localDelta.x / 2, 0));

                        sizeDelta += new Vector2(localDelta.x, 0);
                        break;
                }

                switch (verticalPoint)
                {
                    case DragEdgeVertical.Top:
                        if (transform.Pivot.IsTop())
                            positionDelta += target.parent.GetRelativeDelta(target, new Vector2(0, localDelta.y));
                        else if (transform.Pivot.IsMiddleRow())
                            positionDelta += target.parent.GetRelativeDelta(target, new Vector2(0, localDelta.y / 2));

                        sizeDelta -= new Vector2(0, localDelta.y);
                        break;
                    case DragEdgeVertical.Middle: break;
                    case DragEdgeVertical.Bottom:
                        if (transform.Pivot.IsBottom())
                            positionDelta += target.parent.GetRelativeDelta(target, new Vector2(0, localDelta.y));
                        else if (transform.Pivot.IsMiddleRow())
                            positionDelta += target.parent.GetRelativeDelta(target, new Vector2(0, localDelta.y / 2));

                        sizeDelta += new Vector2(0, localDelta.y);
                        break;
                }


                if (transform.HorizontalPosition == Shared.ScreenData.Types.Unit.Percentage)
                    positionDelta.x = 100 * positionDelta.x / target.parent.contentRect.width;

                if (transform.VerticalPosition == Shared.ScreenData.Types.Unit.Percentage)
                    positionDelta.y = 100 * positionDelta.y / target.parent.contentRect.height;

                if (transform.HorizontalSize == Shared.ScreenData.Types.Unit.Percentage)
                    sizeDelta.x = 100 * sizeDelta.x / target.parent.contentRect.width;

                if (transform.VerticalSize == Shared.ScreenData.Types.Unit.Percentage)
                    sizeDelta.y = 100 * sizeDelta.y / target.parent.contentRect.height;

                transform.Position.x += positionDelta.x;
                transform.Position.y += positionDelta.y;
                transform.Size.x += sizeDelta.x;
                transform.Size.y += sizeDelta.y;

                rendererElement.OnValuesChanged();
                editorState.TriggerElementIsDirty(rendererView, new ValuesUpdated(editorState.SelectedElement));
                WrapToTarget();
            }
        }
    }
}
