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
        public Container(string containerName, string name, Texture2D texture2D)
        {
            Header header = new Header(containerName, name, texture2D);
            header.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-element-header");
            Add(header);

            style.borderBottomColor = Colors.CONTAINER_BORDER_TRIM;
            style.borderBottomWidth = 1;
        }
    }
}
