using JESUIS.Editor.Elements.Common.VisualElements;
using JESUIS.Editor.Helpers.Motions;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy;
using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors;
using UnityEngine;
using UnityEngine.UIElements;
using JESUIS.Editor.Resources;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer
{
    public class RendererDisplay : MaterialRTTVisualElement
    {
        EditorState editorState;
        RendererHierarchyController hierarchyController;

        const float MIN_ZOOM = 0.05f;
        const float MAX_ZOOM = 10f;
        const float ZOOM_SPEED = 0.1f;
        const float ASPECT_RATIO_INIT_PADDING = 150;

        bool isAttached = false; 
        bool isGeometryReady = false;
        DragHelper dragHelper = new DragHelper();
        ZoomHelper zoomHelper = new ZoomHelper();

        Vector2Int? queuedAspectRatioChanged = null;

        float currentWidth = 100;
        float currentHeight = 100;

        public RendererDisplay(EditorState editorState, BoxSelector boxSelector) : base(ResourceLoader.Instance.Shaders.Background.Value)
        {
            this.editorState = editorState;
            hierarchyController = new RendererHierarchyController(editorState, boxSelector);
            Add(hierarchyController);

            style.position = Position.Absolute;
            currentWidth = 100;
            currentHeight = 100;

            RegisterCallback<AttachToPanelEvent>(OnAttach);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            dragHelper.RegisterOnPositionChanged(x => UpdateTransform());
            zoomHelper.RegisterOnChange(OnZoomChanged);

            SetBackgroundColors();
        }

        void SetBackgroundColors()
        {
            Material material = GetMaterial();
            material.SetColor("_Color1", Colors.RENDERER_CHECKERBACKGROUND_LIGHT_COLOR);
            material.SetColor("_Color2", Colors.RENDERER_CHECKERBACKGROUND_DARK_COLOR);
            material.SetFloat("_DivisionsHorizontal", currentWidth / 10);
            material.SetFloat("_DivisionsVertical", currentHeight / 10);
            UpdateTexture();
        }

        public void ChangeAspectRatio(int width, int height)
        {
            if (!isGeometryReady)
            {
                queuedAspectRatioChanged = new Vector2Int(width, height);
                return;
            }

            currentWidth = width; 
            currentHeight = height;

            SetSize(width, height);

            // adjust size to fit panel
            float parentWidth = parent.resolvedStyle.width - ASPECT_RATIO_INIT_PADDING; 
            float parentHeight = parent.resolvedStyle.height - ASPECT_RATIO_INIT_PADDING;
            
            float xScale = (parentWidth / width); 
            float yScale = (parentHeight / height);

            float zoom = Mathf.Min(xScale, yScale);
            zoomHelper.UpdateZoom(zoom, new Vector2(parent.style.width.value.value / 2, parent.style.height.value.value / 2));
            dragHelper.SetOffset(Vector2.zero);
            SetBackgroundColors();
        }

        public void UpdateTransform()
        {
            Vector2 centerOffset = GetCenterOffset();
            this.style.left = centerOffset.x + dragHelper.Offset.x;
            this.style.top = centerOffset.y + dragHelper.Offset.y;
        }

        public void OnZoomChanged(float newZoom, float previousZoom, Vector2 targetMousePosition)
        {
            style.scale = new Vector2(newZoom, newZoom);

            Vector2 relativeMousePosition = this.WorldToLocal(targetMousePosition);
            Vector2 prevSize = new Vector2(currentWidth, currentHeight) * previousZoom;
            Vector2 mousePosition = new Vector2(relativeMousePosition.x / currentWidth - 0.5f, relativeMousePosition.y / currentHeight - 0.5f);

            Vector2 newSize = new Vector2(currentWidth, currentHeight) * newZoom;

            style.scale = new Vector2(newZoom, newZoom);

            Vector2 centerOffset = GetCenterOffset();
            dragHelper.SetOffset
                ( new Vector2
                    ( style.left.value.value + (prevSize.x - newSize.x) * mousePosition.x - centerOffset.x
                    , style.top.value.value + (prevSize.y - newSize.y) * mousePosition.y - centerOffset.y
                    )
                );
        }

        void OnParentGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateTransform();
        } 

        void OnAttach(AttachToPanelEvent evt)
        {
            if (isAttached)
            {
                return;
            }
            isAttached = true;

            parent.RegisterCallback<GeometryChangedEvent>(OnParentGeometryChanged);
            dragHelper.SetTarget(2, parent, true);
            zoomHelper.SetTarget(parent, MIN_ZOOM, MAX_ZOOM, ZOOM_SPEED, ZoomHelper.ZoomMethod.Multiplicative);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (isGeometryReady)
            {
                return;
            }
            isGeometryReady = true;

            if (queuedAspectRatioChanged != null)
            {
                ChangeAspectRatio(queuedAspectRatioChanged.Value.x, queuedAspectRatioChanged.Value.y);
            }
            else
            {
                UpdateTransform();
            }
        }

        Vector2 GetCenterOffset()
        {
            return new Vector2
                ( parent.contentRect.width / 2 - style.width.value.value / 2
                , parent.contentRect.height / 2 - style.height.value.value / 2
                );
        }
    }
}
