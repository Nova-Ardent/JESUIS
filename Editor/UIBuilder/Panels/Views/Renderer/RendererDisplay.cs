using JESUIS.Editor.Elements.Common.VisualElements;
using JESUIS.Editor.Helpers.Motions;
using JESUIS.Editor.Settings;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer
{
    public class RendererDisplay : MaterialRTTVisualElement
    {
        static readonly string BackgroundShaderPath = "Assets/JESUIS/Editor/Resources/Shaders/UIEditor/Renderer/Background.shader";

        const float MIN_ZOOM = 0.05f;
        const float MAX_ZOOM = 10f;
        const float ZOOM_SPEED = 0.1f;

        bool isAttached = false;
        DragHelper dragHelper = new DragHelper();
        ZoomHelper zoomHelper = new ZoomHelper();

        float currentWidth = 100;
        float currentHeight = 100;

        public RendererDisplay() : base(AssetDatabase.LoadAssetAtPath<Shader>(BackgroundShaderPath))
        {
            style.position = Position.Absolute;
            SetSize(100, 100);
            currentWidth = 100;
            currentHeight = 100;

            RegisterCallback<AttachToPanelEvent>(OnAttach);
            dragHelper.RegisterOnPositionChanged(x => UpdateTransform());
            zoomHelper.RegisterOnChange(OnZoomChanged);

            SetBackgroundColors();
        }

        void SetBackgroundColors()
        {
            Material material = GetMaterial();
            material.SetColor("_Color1", Colors.RENDERER_CHECKERBACKGROUND_LIGHT_COLOR);
            material.SetColor("_Color2", Colors.RENDERER_CHECKERBACKGROUND_DARK_COLOR);
            material.SetFloat("_Divisions", 100 / 10);
            UpdateTexture();
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

            parent.RegisterCallback<GeometryChangedEvent>(OnParentGeometryChanged);
            dragHelper.SetTarget(2, parent, true);
            zoomHelper.SetTarget(parent, MIN_ZOOM, MAX_ZOOM, ZOOM_SPEED, ZoomHelper.ZoomMethod.Multiplicative);

            UpdateTransform();
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
