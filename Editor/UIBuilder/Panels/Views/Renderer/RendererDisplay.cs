using JESUIS.Editor.Helpers.Motions;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer
{
    public class RendererDisplay : VisualElement
    {
        bool isAttached = false;
        DragHelper dragHelper = new DragHelper();

        public RendererDisplay()
        {
            style.position = Position.Absolute;
            style.width = 100;
            style.height = 100;
            style.backgroundColor = Color.white;

            RegisterCallback<AttachToPanelEvent>(OnAttach);
            dragHelper.RegisterOnPositionChanged(x => UpdateTransform());
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
