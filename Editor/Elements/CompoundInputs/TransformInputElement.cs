using JESUIS.Editor.Elements.Input.Bindable;
using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Elements.SpecialInputs;
using JESUIS.Editor.Resources;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Panels.Views;
using JESUIS.Editor.Utilities.StyleSheets;
using JESUIS.Shared.ScreenData.Types;
using System.Reflection;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

namespace JESUIS.Editor.Elements.CompoundInputs
{
    public class TransformInputElement : Container
    {
        public const int BOTTOM_PADDING = 6;
        public const int ELEMENT_PADDING = 2;
        public const int ALIGNMENT_PADDING = 20;

        BindableVector2fFieldElement sizeField;
        BindableVector2fFieldElement positionField;
        BindableVector2fFieldElement scaleField;
        BindableFloatInputFieldElement rotationField;

        BindableAlignmentSelector anchorField;
        BindableAlignmentSelector pivotField;

        BindableEnumFieldElement<Unit> verticalPositionField;
        BindableEnumFieldElement<Unit> verticalSizeField;

        BindableEnumFieldElement<Unit> horizontalPositionField;
        BindableEnumFieldElement<Unit> horizontalSizeField;

        Action onChange;

        Shared.ScreenData.Types.Transform targetTransform;

        public TransformInputElement(string name, Shared.ScreenData.Types.Transform target, TransformDatas[] transformDataLocks) : base("Transform", name, ResourceLoader.Instance.Icons.Inspector.Transform.Value)
        {
            targetTransform = target;

            this.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-element");

            sizeField = new BindableVector2fFieldElement("Size", "W", "H");
            sizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-size");
            sizeField.SetWithoutNotify(target.Size);
            sizeField.RegisterOnValueChanged((newValue) =>
            {
                target.Size = newValue;
                onChange?.Invoke();
            });
            sizeField.Lock(transformDataLocks.Contains(TransformDatas.Size));
            Add(sizeField);

            positionField = new BindableVector2fFieldElement("Position");
            positionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-position");
            positionField.SetWithoutNotify(target.Position);
            positionField.RegisterOnValueChanged((newValue) =>
            {
                target.Position = newValue;
                onChange?.Invoke();
            });
            positionField.Lock(transformDataLocks.Contains(TransformDatas.Position));
            Add(positionField);

            scaleField = new BindableVector2fFieldElement("Scale");
            scaleField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-scale");
            scaleField.SetWithoutNotify(target.Scale);
            scaleField.RegisterOnValueChanged((newValue) =>
            {
                target.Scale = newValue;
                onChange?.Invoke();
            });
            scaleField.Lock(transformDataLocks.Contains(TransformDatas.Scale));
            Add(scaleField);

            rotationField = new BindableFloatInputFieldElement("Rotation", 0);
            rotationField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-rotation");
            rotationField.SetWithoutNotify(target.Rotation);
            rotationField.RegisterOnValueChanged((newValue) =>
            {
                target.Rotation = newValue;
                onChange?.Invoke();
            });
            rotationField.Lock(transformDataLocks.Contains(TransformDatas.Rotation));
            Add(rotationField);

            anchorField = new BindableAlignmentSelector("Anchor");
            anchorField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-anchor");
            anchorField.SetWithoutNotify(target.Anchor);
            anchorField.RegisterOnValueChanged((newValue) =>
            {
                target.Anchor = newValue;
                onChange?.Invoke();
            });
            anchorField.Lock(transformDataLocks.Contains(TransformDatas.Anchor));
            Add(anchorField);

            pivotField = new BindableAlignmentSelector("Pivot");
            pivotField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-pivot");
            pivotField.SetWithoutNotify(target.Pivot);
            pivotField.RegisterOnValueChanged((newValue) =>
            {
                target.Pivot = newValue;
                onChange?.Invoke();
            });
            pivotField.Lock(transformDataLocks.Contains(TransformDatas.Pivot));
            Add(pivotField);

            verticalPositionField = new BindableEnumFieldElement<Unit>("Vert Pos", Unit.Pixels);
            verticalPositionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-vertical-position");
            verticalPositionField.SetWithoutNotify(target.VerticalPosition);
            verticalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalPosition = newValue;
                onChange?.Invoke();
            });
            verticalPositionField.Lock(transformDataLocks.Contains(TransformDatas.VerticalPosition));
            Add(verticalPositionField);

            verticalSizeField = new BindableEnumFieldElement<Unit>("Vert Size", Unit.Pixels);
            verticalSizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-vertical-size");
            verticalSizeField.SetValueWithoutNotify(target.VerticalSize.Value);
            verticalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.VerticalSize = newValue;
                onChange?.Invoke();
            });
            verticalSizeField.Lock(transformDataLocks.Contains(TransformDatas.VerticalSize));
            Add(verticalSizeField);

            horizontalPositionField = new BindableEnumFieldElement<Unit>("Horz Pos", Unit.Pixels);
            horizontalPositionField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-horizontal-position");
            horizontalPositionField.SetWithoutNotify(target.HorizontalPosition);
            horizontalPositionField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalPosition = newValue;
                onChange?.Invoke();
            });
            horizontalPositionField.Lock(transformDataLocks.Contains(TransformDatas.HorizontalPosition));
            Add(horizontalPositionField);

            horizontalSizeField = new BindableEnumFieldElement<Unit>("Horz Size", Unit.Pixels);
            horizontalSizeField.AddStyle(TransformInputElementUSS.StyleSheetInstance, "transform-horizontal-size"); 
            horizontalSizeField.SetWithoutNotify(target.HorizontalSize);
            horizontalSizeField.RegisterOnValueChanged((newValue) =>
            {
                target.HorizontalSize = newValue;
                onChange?.Invoke();
            });
            horizontalSizeField.Lock(transformDataLocks.Contains(TransformDatas.HorizontalSize));
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
            sizeField.SetValueWithoutNotify(targetTransform.Size.Value.x, targetTransform.Size.Value.y);
            positionField.SetValueWithoutNotify(targetTransform.Position.Value.x, targetTransform.Position.Value.y);
            scaleField.SetValueWithoutNotify(targetTransform.Scale.Value.x, targetTransform.Scale.Value.y);
            rotationField.SetValueWithoutNotify(targetTransform.Rotation.Value);

            anchorField.SetValueWithoutNotify(targetTransform.Anchor.Value);
            pivotField.SetValueWithoutNotify(targetTransform.Pivot.Value);

            verticalPositionField.SetValueWithoutNotify(targetTransform.VerticalPosition.Value);
            verticalSizeField.SetValueWithoutNotify(targetTransform.VerticalSize.Value);
            horizontalPositionField.SetValueWithoutNotify(targetTransform.HorizontalPosition.Value);
            horizontalSizeField.SetValueWithoutNotify(targetTransform.HorizontalSize.Value);  
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            verticalPositionField.style.width = contentRect.width - verticalPositionField.resolvedStyle.left + 10;
            verticalSizeField.style.width = contentRect.width - verticalSizeField.resolvedStyle.left + 10;
            horizontalPositionField.style.width = contentRect.width - horizontalPositionField.resolvedStyle.left + 10;
            horizontalSizeField.style.width = contentRect.width - horizontalSizeField.resolvedStyle.left + 10;
        }

        public static TransformInputElement RegisterField(FieldInfo info, object target, EditorViews triggeringView, EditorState editorState, ref Action onSelectedElementUpdated, TransformElementLock[] transformLocks)
        {
            Shared.ScreenData.Types.Transform transform = (Shared.ScreenData.Types.Transform)info.GetValue(target);
            if (transform == null)
            {
                Debug.LogError("transform element is null.");
                return null;
            }

            TransformInputElement inputElement = new TransformInputElement(info.Name, transform, transformLocks
                .SelectMany(x => x.LockedDatas)
                .Distinct()
                .ToArray());
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
