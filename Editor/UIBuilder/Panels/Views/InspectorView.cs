using JESUIS.Editor.Elements.Common.Input;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Shared.ScreenData.ScreenDataTypes;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;


namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class InspectorView : EditorViews
    {
        EditorState editorState;

        public override Views Type => Views.Inspector;

        public InspectorView(EditorState editorState)
        {
            this.editorState = editorState;
            editorState.SelectedElement.ListenTo(InspectingNewElement);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
        }

        public void InspectingNewElement(BaseElement baseElement)
        {
            Clear();
            if (baseElement is RootElement)
            {
                return;
            }

            float CurrentPosition = 0;
            foreach (var field in GetAllFields(baseElement.GetType()).DistinctBy(x => x.Name))
            {
                VisualElement visualElement = GetInspectorElement(field, baseElement);
                if (visualElement == null)
                    continue;

                visualElement.style.position = Position.Absolute;
                visualElement.style.left = 0;
                visualElement.style.top = CurrentPosition;
                Add(visualElement);

                CurrentPosition += visualElement.style.height.value.value;
            }
        }

        public VisualElement GetInspectorElement(FieldInfo fieldInfo, object target)
        {
            switch (fieldInfo.FieldType)
            {
                case var type when type == typeof(string): return RegisterStringInputField(fieldInfo, target);
                case var type when type == typeof(int): return RegisterIntInputField(fieldInfo, target);
                case var type when type == typeof(float): return RegisterFloatInputField(fieldInfo, target);
                case var type when type == typeof(Vector2): return Vector2fFieldElement(fieldInfo, target);
                case var type when type == typeof(Vector2Int): return Vector2iFieldElement(fieldInfo, target);
                default:
                    Debug.LogWarning($"Could not create inspector element for field type {fieldInfo.FieldType}");
                    return null;
            }
        }

        IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            while (type != null)
            {
                foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (fieldInfo.IsPublic)
                    {
                        yield return fieldInfo;
                        continue;
                    }

                    if (fieldInfo.IsDefined(typeof(SerializeField), true))
                    {
                        yield return fieldInfo;
                        continue;
                    }
                }

                type = type.BaseType;
            }
        }

        VisualElement RegisterStringInputField(FieldInfo info, object target)
        {
            TextInputFieldElement textField = new TextInputFieldElement(info.Name, "");
            textField.SetValueWithoutNotify(info.GetValue(target)?.ToString() ?? "");
            textField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                editorState.TriggerSelectedElementIsDirty(this);
            });
            return textField;
        }

        VisualElement RegisterIntInputField(FieldInfo info, object target)
        {
            IntInputFieldElement intField = new IntInputFieldElement(info.Name, 0);
            intField.SetValueWithoutNotify((int)info.GetValue(target));
            intField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                editorState.TriggerSelectedElementIsDirty(this);
            });
            return intField;
        }

        VisualElement RegisterFloatInputField(FieldInfo info, object target)
        {
            FloatInputFieldElement floatField = new FloatInputFieldElement(info.Name, 0f);
            floatField.SetValueWithoutNotify((float)info.GetValue(target));
            floatField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                editorState.TriggerSelectedElementIsDirty(this);
            });
            return floatField;
        }

        VisualElement Vector2fFieldElement(FieldInfo info, object target)
        {
            Vector2fFieldElement vectorField = new Vector2fFieldElement(info.Name);
            vectorField.SetValueWithoutNotify((Vector2)info.GetValue(target));
            vectorField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                editorState.TriggerSelectedElementIsDirty(this);
            });
            return vectorField;
        }

        VisualElement Vector2iFieldElement(FieldInfo info, object target)
        {
            Vector2iFieldElement vectorField = new Vector2iFieldElement(info.Name);
            vectorField.SetValueWithoutNotify((Vector2Int)info.GetValue(target));
            vectorField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                editorState.TriggerSelectedElementIsDirty(this);
            });
            return vectorField;
        }
    }
}
