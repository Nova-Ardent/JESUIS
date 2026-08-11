using JESUIS.Editor.Elements.Display;
using JESUIS.Editor.Resources;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.DragPoints;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.Edges;
using JESUIS.Shared.ScreenData.Types;
using UnityEngine.UIElements;
using UnityEngine;

using static JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.DragPoints.DragPoint;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors
{
    public class BoxSelector : VisualElement
    {
        BoxSelectorEdge[] boxSelectorEdges = new BoxSelectorEdge[4];
        DragPoint[,] dragPoints = new DragPoint[3, 3];
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
            style.left = 0;
            style.top = 0;
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

            for (int i = 0; i < 4; i++)
            {
                boxSelectorEdges[i] = new BoxSelectorEdge();
                Add(boxSelectorEdges[i]);
            }

            AddDragPoint(ref dragPoints[0, 0], 45f, DragEdgeHorizontal.Left, DragEdgeVertical.Top, mouseContainer);
            AddDragPoint(ref dragPoints[2, 0], -45f, DragEdgeHorizontal.Right, DragEdgeVertical.Top, mouseContainer);
            AddDragPoint(ref dragPoints[0, 2], -45f, DragEdgeHorizontal.Left, DragEdgeVertical.Bottom, mouseContainer);
            AddDragPoint(ref dragPoints[2, 2], 45f, DragEdgeHorizontal.Right, DragEdgeVertical.Bottom, mouseContainer);

            AddDragPoint(ref dragPoints[1, 0], 90f, DragEdgeHorizontal.Middle, DragEdgeVertical.Top, mouseContainer);
            AddDragPoint(ref dragPoints[0, 1], 0f, DragEdgeHorizontal.Left, DragEdgeVertical.Middle, mouseContainer);
            AddDragPoint(ref dragPoints[2, 1], 0f, DragEdgeHorizontal.Right, DragEdgeVertical.Middle, mouseContainer);
            AddDragPoint(ref dragPoints[1, 2], 90f, DragEdgeHorizontal.Middle, DragEdgeVertical.Bottom, mouseContainer);

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
            if (target == null)
                return;
        
            if (target is IRendererElement rendererElement)
            {
                Shared.ScreenData.Types.Transform transform = rendererElement.GetTransform();

                Vector2 localPosition = transform.GetLocalScaledPosition();
                Vector2 scaledSize = transform.GetScaledLocalSize();
                Vector2 pivot = transform.GetLocalScaledPivot();
                Vector2 transformOrigin = new Vector2(localPosition.x + pivot.x, localPosition.y + pivot.y);

                Vector2[,] cornerPositions = new Vector2[,]
                {
                    {
                        new Vector2(localPosition.x, localPosition.y),
                        new Vector2(localPosition.x, localPosition.y + scaledSize.y),
                    },
                    {
                        new Vector2(localPosition.x + scaledSize.x, localPosition.y),
                        new Vector2(localPosition.x + scaledSize.x, localPosition.y + scaledSize.y),
                    }
                };

                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        Vector2 rotatedPoint = cornerPositions[x, y].RotatePoint(transformOrigin, rendererElement.GetTransform().Rotation);
                        cornerPositions[x, y] = this.WorldToLocal(target.parent.LocalToWorld(rotatedPoint));

                        dragPoints[x * 2, y * 2].SetPosition(cornerPositions[x, y]);
                    }
                }

                dragPoints[1, 0].SetPosition((cornerPositions[0, 0] + cornerPositions[1, 0]) / 2);
                dragPoints[0, 1].SetPosition((cornerPositions[0, 0] + cornerPositions[0, 1]) / 2);
                dragPoints[2, 1].SetPosition((cornerPositions[1, 0] + cornerPositions[1, 1]) / 2);
                dragPoints[1, 2].SetPosition((cornerPositions[0, 1] + cornerPositions[1, 1]) / 2);

                Shared.ScreenData.Types.Transform rotationIter = transform;
                float rotation = 0;
                do
                {
                    rotation += rotationIter.Rotation;
                    rotationIter = rotationIter.parent;
                } while (rotationIter != null);

                foreach (DragPoint dragPoint in dragPoints)
                {
                    if (dragPoint != null)
                        dragPoint.SetDragCursorAngle(rotation);
                }
                
                rotationPoint.style.rotate = new Rotate(new Angle(rotation, AngleUnit.Degree));
                rotationPoint.SetCursorAngle(rotation);

                boxSelectorEdges[0].SetEdgeData(cornerPositions[0, 0], cornerPositions[1, 0]);
                boxSelectorEdges[1].SetEdgeData(cornerPositions[0, 0], cornerPositions[0, 1]);
                boxSelectorEdges[2].SetEdgeData(cornerPositions[1, 0], cornerPositions[1, 1]);
                boxSelectorEdges[3].SetEdgeData(cornerPositions[0, 1], cornerPositions[1, 1]);

                switch (transform.Pivot)
                {
                    case Alignment.TopLeft:     rotationPoint.SetPosition(cornerPositions[0, 0]); break;
                    case Alignment.Top:         rotationPoint.SetPosition((cornerPositions[0, 0] + cornerPositions[1, 0]) / 2); break;
                    case Alignment.TopRight:    rotationPoint.SetPosition(cornerPositions[1, 0]); break;
                    case Alignment.Left:        rotationPoint.SetPosition((cornerPositions[0, 0] + cornerPositions[0, 1]) / 2); break;
                    case Alignment.Middle:      rotationPoint.SetPosition((cornerPositions[0, 0] + cornerPositions[1, 1]) / 2); break;
                    case Alignment.Right:       rotationPoint.SetPosition((cornerPositions[1, 0] + cornerPositions[1, 1]) / 2); break;
                    case Alignment.BottomLeft:  rotationPoint.SetPosition(cornerPositions[0, 1]); break;
                    case Alignment.Bottom:      rotationPoint.SetPosition((cornerPositions[0, 1] + cornerPositions[1, 1]) / 2); break;
                    case Alignment.BottomRight: rotationPoint.SetPosition(cornerPositions[1, 1]); break;
                }
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
