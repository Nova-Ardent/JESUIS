using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Elements.SpecialInputs;
using JESUIS.Editor.Settings;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Panels.Views;
using JESUIS.Editor.Utilities.StyleSheets;
using JESUIS.Shared.ScreenData.Types;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using JESUIS.Editor.Resources;

namespace JESUIS.Editor.Elements.CompoundInputs
{
    public class TransformInputElement : Container
    {
        public const int BOTTOM_PADDING = 6;
        public const int ELEMENT_PADDING = 2;
        public const int ALIGNMENT_PADDING = 20;

        Vector2fFieldElement sizeField;
        Vector2fFieldElement positionField;
        Vector2fFieldElement scaleField;
        FloatInputFieldElement rotationField;

        AlignmentSelector anchorField;
        AlignmentSelector pivotField;

        EnumFieldElement<Unit> verticalPositionField;
        EnumFieldElement<Unit> verticalSizeField;

        EnumFieldElement<Unit> horizontalPositionField;
        EnumFieldElement<Unit> horizontalSizeField;

        Action onChange;

        Shared.ScreenData.Types.Transform targetTransform;

        public TransformInputElement(string name, Shared.ScreenData.Types.Transform target) : base("Transform", name)
        {
            targetTransform = target;

            this.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-element");

            sizeField = new Vector2fFieldElement("Size", "W", "H");
            sizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-size");
            sizeField.SetValuesWithoutNotify(target.Size.x, target.Size.y);
            sizeField.RegisterOnValueChanged((newValue) =>
            {
                target.Size = newValue;
                onChange?.Invoke();
            });
            Add(sizeField);

            positionField = new Vector2fFieldElement("Position");
            positionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-position");
            positionField.SetValuesWithoutNotify(target.Position.x, target.Position.y);
            positionField.RegisterOnValueChanged((newValue) =>
            {
                target.Position = newValue;
                onChange?.Invoke();
            });
            Add(positionField);

            scaleField = new Vector2fFieldElement("Scale");
            scaleField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-scale");
            scaleField.SetValuesWithoutNotify(target.Scale.x, target.Scale.y);
            scaleField.RegisterOnValueChanged((newValue) =>
            {
                target.Scale = newValue;
                onChange?.Invoke();
            });
            Add(scaleField);

            rotationField = new FloatInputFieldElement("Rotation", 0, false);
            rotationField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-rotation");
            rotationField.SetValueWithoutNotify(target.Rotation);
            rotationField.RegisterOnValueChanged((newValue) =>
            {
                target.Rotation = newValue;
                onChange?.Invoke();
            });
            Add(rotationField);

            anchorField = new AlignmentSelector("Anchor");
            anchorField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-anchor");
            anchorField.SetValueWithoutNotify(target.Anchor);
            anchorField.RegisterOnValueChanged((newValue) =>
            {
                target.Anchor = newValue;
                onChange?.Invoke();
            });
            Add(anchorField);

            pivotField = new AlignmentSelector("Pivot");
            pivotField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-pivot");
            pivotField.SetValueWithoutNotify(target.Pivot);
            pivotField.RegisterOnValueChanged((newValue) =>
            {
                target.Pivot = newValue;
                onChange?.Invoke();
            });
            Add(pivotField);

            verticalPositionField = new EnumFieldElement<Unit>("Vert Pos", Unit.Pixels);
            verticalPositionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-vertical-position");
            verticalPositionField.SetValueWithoutNotify(target.VerticalPosition);
            verticalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalPosition = newValue;
                onChange?.Invoke();
            });
            Add(verticalPositionField);

            verticalSizeField = new EnumFieldElement<Unit>("Vert Size", Unit.Pixels);
            verticalSizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-vertical-size");
            verticalSizeField.SetValueWithoutNotify(target.VerticalSize);
            verticalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalSize = newValue;
                onChange?.Invoke();
            });
            Add(verticalSizeField);

            horizontalPositionField = new EnumFieldElement<Unit>("Horz Pos", Unit.Pixels);
            horizontalPositionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-horizontal-position");
            horizontalPositionField.SetValueWithoutNotify(target.HorizontalPosition);
            horizontalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalPosition = newValue;
                onChange?.Invoke();
            });
            Add(horizontalPositionField);

            horizontalSizeField = new EnumFieldElement<Unit>("Horz Size", Unit.Pixels);
            horizontalSizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-horizontal-size"); 
            horizontalSizeField.SetValueWithoutNotify(target.HorizontalSize);
            horizontalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalSize = newValue;
                onChange?.Invoke();
            });
            Add(horizontalSizeField);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void RegisterOnValueChanged(Action onChange)
        {
            if (this.onChange == null)
            {
                this.onChange = onChange;
            }
            else
            {
                this.onChange += onChange;
            }
        }

        public void UpdateInspectorElements()
        {
            sizeField.SetValuesWithoutNotify(targetTransform.Size.x, targetTransform.Size.y);
            positionField.SetValuesWithoutNotify(targetTransform.Position.x, targetTransform.Position.y);
            scaleField.SetValuesWithoutNotify(targetTransform.Scale.x, targetTransform.Scale.y);
            rotationField.SetValueWithoutNotify(targetTransform.Rotation);
            anchorField.SetValueWithoutNotify(targetTransform.Anchor);
            pivotField.SetValueWithoutNotify(targetTransform.Pivot);
            verticalPositionField.SetValueWithoutNotify(targetTransform.VerticalPosition);
            verticalSizeField.SetValueWithoutNotify(targetTransform.VerticalSize);
            horizontalPositionField.SetValueWithoutNotify(targetTransform.HorizontalPosition);
            horizontalSizeField.SetValueWithoutNotify(targetTransform.HorizontalSize);  
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            verticalPositionField.style.width = contentRect.width - verticalPositionField.resolvedStyle.left;
            verticalSizeField.style.width = contentRect.width - verticalSizeField.resolvedStyle.left;
            horizontalPositionField.style.width = contentRect.width - horizontalPositionField.resolvedStyle.left;
            horizontalSizeField.style.width = contentRect.width - horizontalSizeField.resolvedStyle.left;
        }

        public static TransformInputElement RegisterField(FieldInfo info, object target, EditorViews triggeringView, EditorState editorState, ref Action onSelectedElementUpdated)
        {
            Shared.ScreenData.Types.Transform transform = (Shared.ScreenData.Types.Transform)info.GetValue(target);
            if (transform == null)
            {
                Debug.LogError("transform element is null.");
                return null;
            }

            TransformInputElement inputElement = new TransformInputElement(info.Name, transform);
            inputElement.RegisterOnValueChanged(() =>
            {
                editorState.TriggerElementIsDirty(triggeringView, new ValuesUpdated(editorState.SelectedElement));
            });

            if (onSelectedElementUpdated == null)
            {
                onSelectedElementUpdated = inputElement.UpdateInspectorElements;
            }
            else
            {
                onSelectedElementUpdated += inputElement.UpdateInspectorElements;
            }
            return inputElement;
        }
    }
}
