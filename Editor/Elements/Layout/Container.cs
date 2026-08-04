using JESUIS.Editor.Elements.CompoundInputs;
using JESUIS.Editor.Resources;
using JESUIS.Editor.Utilities.StyleSheets;
using UnityEngine.UIElements;
using UnityEngine;
using JESUIS.Editor.Settings;

namespace JESUIS.Editor.Elements.Layout
{
    public class Container : VisualElement
    {
        public Container(string containerName, string name)
        {
            Header header = new Header(containerName, name, ResourceLoader.Instance.Icons.Inspector.Transform.Value);
            header.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-element-header");
            Add(header);

            style.borderBottomColor = Colors.CONTAINER_BORDER_TRIM;
            style.borderBottomWidth = 1;
        }
    }
}
