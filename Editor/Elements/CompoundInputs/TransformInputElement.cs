using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Elements.SpecialInputs;
using JESUIS.Editor.Settings;
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
            style.width = Length.Percent(100);
            style.borderBottomWidth = 1;
            style.borderBottomColor = Colors.TRANSFORM_INPUT_BORDER_TRIM;

            Header header = new Header("Transform", name, AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath));
            header.style.position = Position.Absolute;
            header.style.top = 0;
            Add(header);

            positionField = new Vector2fFieldElement("Position");
            positionField.style.position = Position.Absolute;
            positionField.style.top = Header.HEADER_HEIGHT + ELEMENT_PADDING;
            positionField.style.left = 0;
            positionField.SetValuesWithoutNotify(target.Position.x, target.Position.y);
            positionField.RegisterOnValueChanged((newValue) =>
            {
                target.Position = newValue;
            });
            Add(positionField);

            scaleField = new Vector2fFieldElement("Scale");
            scaleField.style.position = Position.Absolute;
            scaleField.style.top = positionField.style.height.value.value + ELEMENT_PADDING + positionField.style.top.value.value;
            scaleField.style.left = 0;
            scaleField.SetValuesWithoutNotify(target.Size.x, target.Size.y);
            scaleField.RegisterOnValueChanged((newValue) =>
            {
                target.Size = newValue;
            });
            Add(scaleField);

            rotationField = new FloatInputFieldElement("Rotation", target.Rotation, false);
            rotationField.style.position = Position.Absolute;
            rotationField.style.top = scaleField.style.height.value.value + ELEMENT_PADDING + scaleField.style.top.value.value;
            rotationField.style.left = 0;
            rotationField.SetValueWithoutNotify(target.Rotation);
            rotationField.RegisterOnValueChanged((newValue) =>
            {
                target.Rotation = newValue;
            });
            Add(rotationField);

            anchorField = new AlignmentSelector("Anchor");
            anchorField.style.position = Position.Absolute;
            anchorField.style.top = rotationField.style.height.value.value + ELEMENT_PADDING + rotationField.style.top.value.value;
            anchorField.style.left = 20;
            anchorField.SetValueWithoutNotify(target.Anchor);
            anchorField.RegisterOnValidChanged((newValue) =>
            {
                target.Anchor = newValue;
            });
            Add(anchorField);

            pivotField = new AlignmentSelector("Pivot");
            pivotField.style.position = Position.Absolute;
            pivotField.style.top = rotationField.style.height.value.value + ELEMENT_PADDING + rotationField.style.top.value.value;
            pivotField.style.left = 20 + anchorField.style.width.value.value + ALIGNMENT_PADDING;
            pivotField.SetValueWithoutNotify(target.Pivot);
            pivotField.RegisterOnValidChanged((newValue) =>
            {
                target.Pivot = newValue;
            });
            Add(pivotField);

            verticalPositionField = new EnumFieldElement<Unit>("Vert Pos", Unit.Pixels);
            verticalPositionField.style.position = Position.Absolute;
            verticalPositionField.style.top = rotationField.style.height.value.value + ELEMENT_PADDING + rotationField.style.top.value.value;
            verticalPositionField.style.right = ELEMENT_PADDING;
            verticalPositionField.style.width = Length.Percent(60);
            verticalPositionField.SetValueWithoutNotify(target.VerticalPosition);
            verticalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalPosition = newValue;
            });
            Add(verticalPositionField);

            verticalSizeField = new EnumFieldElement<Unit>("Vert Size", Unit.Pixels);
            verticalSizeField.style.position = Position.Absolute;
            verticalSizeField.style.top = verticalPositionField.style.height.value.value + ELEMENT_PADDING + verticalPositionField.style.top.value.value;
            verticalSizeField.style.right = ELEMENT_PADDING;
            verticalSizeField.style.width = Length.Percent(60);
            verticalSizeField.SetValueWithoutNotify(target.VerticalSize);
            verticalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalSize = newValue;
            });
            Add(verticalSizeField);

            horizontalPositionField = new EnumFieldElement<Unit>("Horz Pos", Unit.Pixels);
            horizontalPositionField.style.position = Position.Absolute;
            horizontalPositionField.style.top = verticalSizeField.style.height.value.value + ELEMENT_PADDING + verticalSizeField.style.top.value.value;
            horizontalPositionField.style.right = ELEMENT_PADDING;
            horizontalPositionField.style.width = Length.Percent(60);
            horizontalPositionField.SetValueWithoutNotify(target.HorizontalPosition);
            horizontalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalPosition = newValue;
            });
            Add(horizontalPositionField);

            horizontalSizeField = new EnumFieldElement<Unit>("Horz Size", Unit.Pixels);
            horizontalSizeField.style.position = Position.Absolute;
            horizontalSizeField.style.top = horizontalPositionField.style.height.value.value + ELEMENT_PADDING + horizontalPositionField.style.top.value.value;
            horizontalSizeField.style.right = ELEMENT_PADDING;
            horizontalSizeField.style.width = Length.Percent(60);
            horizontalSizeField.SetValueWithoutNotify(target.HorizontalSize);
            horizontalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalSize = newValue;
            });
            Add(horizontalSizeField);

            style.height = horizontalSizeField.style.height.value.value + BOTTOM_PADDING + horizontalSizeField.style.top.value.value;
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
