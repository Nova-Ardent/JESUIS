using JESUIS.Editor.Elements.Common.VisualElements;
using JESUIS.Editor.Helpers.Motions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using JESUIS.Editor.Settings;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer
{
    public class RendererDisplay : MaterialRTTVisualElement
    {
        static readonly string BackgroundShaderPath = "Assets/JESUIS/Editor/Resources/Shaders/UIEditor/Renderer/Background.shader";

        bool isAttached = false;
        DragHelper dragHelper = new DragHelper();

        public RendererDisplay() : base(AssetDatabase.LoadAssetAtPath<Shader>(BackgroundShaderPath))
        {
            style.position = Position.Absolute;
            SetSize(100, 100);

            RegisterCallback<AttachToPanelEvent>(OnAttach);
            dragHelper.RegisterOnPositionChanged(x => UpdateTransform());

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
