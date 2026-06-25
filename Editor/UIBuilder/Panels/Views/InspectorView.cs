using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Shared.ScreenData.ScreenDataTypes;
using UnityEngine.UIElements;
using JESUIS.Editor.Elements.Common.Input;
using System.Reflection;
using UnityEngine;
using System;
using System.Collections.Generic;

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
            foreach (var field in GetAllFields(baseElement.GetType()))
            {
                VisualElement visualElement = GetInspectorElement(field, baseElement);
                if (visualElement == null)
                    continue;

                visualElement.style.position = Position.Absolute;
                visualElement.style.left = 0;
                visualElement.style.top = CurrentPosition;
                Add(visualElement);

                CurrentPosition = visualElement.style.height.value.value;
            }
        }

        public VisualElement GetInspectorElement(FieldInfo fieldInfo, object target)
        {
            switch (fieldInfo.FieldType)
            {
                case var type when type == typeof(string):
                    {
                        TextInputFieldElement textField = new TextInputFieldElement(fieldInfo.Name, "");
                        textField.RegisterOnValueChanged(newText =>
                        {
                            fieldInfo.SetValue(target, newText);
                        });
                        return textField;
                    }
                case var type when type == typeof(int):
                    {
                        IntInputFieldElement intField = new IntInputFieldElement(fieldInfo.Name, 0);
                        intField.RegisterOnValueChanged(newText =>
                        {
                            fieldInfo.SetValue(target, newText);
                        });
                        return intField;
                    }
                case var type when type == typeof(float):
                    {
                        FloatInputFieldElement floatField = new FloatInputFieldElement(fieldInfo.Name, 0f);
                        floatField.RegisterOnValueChanged(newText =>
                        {
                            fieldInfo.SetValue(target, newText);
                        });
                        return floatField;
                    }
                default:
                    Debug.LogWarning($"Could not create inspector element for field type {fieldInfo.FieldType}");
                    return null;
            }
        }

        IEnumerable<FieldInfo> GetAllFields(Type type)
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
        }
    }
}
