using JESUIS.Editor.Settings;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Selectors.Edges
{
    public class BoxSelectorEdge : VisualElement
    {
        VisualElement bar = new VisualElement();

        public BoxSelectorEdge()
        {
            bar.style.position = Position.Absolute;
            bar.style.backgroundColor = Colors.RENDERER_BOX_SELECTOR_COLOR;
            bar.style.left = 0;
            bar.style.top = 0;
            bar.style.width = 3;
            bar.style.height = 3;
            Add(bar);

            style.backgroundColor = Colors.RENDERER_BOX_SELECTOR_COLOR;
            style.position = Position.Absolute;
            style.height = 3;
            style.width = 3;
        }

        public void SetEdgeData(Vector2 start, Vector2 end)
        {
            bar.style.height = Vector2.Distance(start, end);

            style.rotate = new Rotate(new Angle(end.GetAngleDegrees(start) + 90));
            style.left = start.x - 1;
            style.top = start.y - 1;
        }
    }
}
