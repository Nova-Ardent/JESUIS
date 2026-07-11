using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Elements.SpecialInputs;
using JESUIS.Editor.Settings;
using JESUIS.Editor.Utilities.StyleSheets;
using JESUIS.Shared.ScreenData.Types;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.CompoundInputs
{
    public class TransformInputElement : VisualElement
    {
        public const int BOTTOM_PADDING = 6;
        public const int ELEMENT_PADDING = 2;
        public const int ALIGNMENT_PADDING = 20;
        public const string IconPath = "Assets/JESUIS/Editor/Resources/Icons/Inspector/Transform.png";

        Vector2fFieldElement positionField;
        Vector2fFieldElement scaleField;
        FloatInputFieldElement rotationField;

        AlignmentSelector anchorField;
        AlignmentSelector pivotField;

        EnumFieldElement<Unit> verticalPositionField;
        EnumFieldElement<Unit> verticalSizeField;

        EnumFieldElement<Unit> horizontalPositionField;
        EnumFieldElement<Unit> horizontalSizeField;
        
        public TransformInputElement(string name, Shared.ScreenData.Types.Transform target)
        {
            this.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-element");
            style.borderBottomColor = Colors.TRANSFORM_INPUT_BORDER_TRIM;

            Header header = new Header("Transform", name, AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath));
            header.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-element-header");
            Add(header);

            positionField = new Vector2fFieldElement("Position");
            positionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-position");
            positionField.SetValuesWithoutNotify(target.Position.x, target.Position.y);
            positionField.RegisterOnValueChanged((newValue) =>
            {
                target.Position = newValue;
            });
            Add(positionField);

            scaleField = new Vector2fFieldElement("Scale");
            scaleField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-scale");
            scaleField.SetValuesWithoutNotify(target.Size.x, target.Size.y);
            scaleField.RegisterOnValueChanged((newValue) =>
            {
                target.Size = newValue;
            });
            Add(scaleField);

            rotationField = new FloatInputFieldElement("Rotation", 0, false);
            rotationField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-rotation");
            rotationField.SetValueWithoutNotify(target.Rotation);
            rotationField.RegisterOnValueChanged((newValue) =>
            {
                target.Rotation = newValue;
            });
            Add(rotationField);

            anchorField = new AlignmentSelector("Anchor");
            anchorField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-anchor");
            anchorField.SetValueWithoutNotify(target.Anchor);
            anchorField.RegisterOnValidChanged((newValue) =>
            {
                target.Anchor = newValue;
            });
            Add(anchorField);

            pivotField = new AlignmentSelector("Pivot");
            pivotField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-pivot");
            pivotField.SetValueWithoutNotify(target.Pivot);
            pivotField.RegisterOnValidChanged((newValue) =>
            {
                target.Pivot = newValue;
            });
            Add(pivotField);


            verticalPositionField = new EnumFieldElement<Unit>("Vert Pos", Unit.Pixels);
            verticalPositionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-vertical-position");
            verticalPositionField.SetValueWithoutNotify(target.VerticalPosition);
            verticalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalPosition = newValue;
            });
            Add(verticalPositionField);

            verticalSizeField = new EnumFieldElement<Unit>("Vert Size", Unit.Pixels);
            verticalSizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-vertical-size");
            verticalSizeField.SetValueWithoutNotify(target.VerticalSize);
            verticalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalSize = newValue;
            });
            Add(verticalSizeField);

            horizontalPositionField = new EnumFieldElement<Unit>("Horz Pos", Unit.Pixels);
            horizontalPositionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-horizontal-position");
            horizontalPositionField.SetValueWithoutNotify(target.HorizontalPosition);
            horizontalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalPosition = newValue;
            });
            Add(horizontalPositionField);

            horizontalSizeField = new EnumFieldElement<Unit>("Horz Size", Unit.Pixels);
            horizontalSizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-horizontal-size"); 
            horizontalSizeField.SetValueWithoutNotify(target.HorizontalSize);
            horizontalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalSize = newValue;
            });
            Add(horizontalSizeField);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            verticalPositionField.style.width = contentRect.width - verticalPositionField.resolvedStyle.left;
            verticalSizeField.style.width = contentRect.width - verticalSizeField.resolvedStyle.left;
            horizontalPositionField.style.width = contentRect.width - horizontalPositionField.resolvedStyle.left;
            horizontalSizeField.style.width = contentRect.width - horizontalSizeField.resolvedStyle.left;
        }

        public static TransformInputElement RegisterField(FieldInfo info, object target)
        {
            Shared.ScreenData.Types.Transform transform = (Shared.ScreenData.Types.Transform)info.GetValue(target);
            if (transform == null)
            {
                Debug.LogError("");
                return null;
            }

            return new TransformInputElement(info.Name, transform);
        }
    }
}
