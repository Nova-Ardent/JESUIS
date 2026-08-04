using JESUIS.Editor.Elements.CompoundInputs;
using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Shared.ScreenData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;


namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class InspectorView : EditorViews
    {
        const int ELEMENT_PADDING = 2;

        Action onSelectedElementUpdated;
        EditorState editorState;

        public override Views Type => Views.Inspector;

        public InspectorView(EditorState editorState)
        {
            this.editorState = editorState;
            editorState.SelectedElement.ListenTo(InspectingNewElement);
            editorState.ListenToElementIsDirty(OnElementIsDirty);

            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
        }

        void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (triggeringView.Type == Views.Inspector)
            {
                return;
            }

            if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ValueUpdated)
            {
                onSelectedElementUpdated?.Invoke();
            }
        }

        void InspectingNewElement(BaseElement baseElement)
        {
            Clear();
            onSelectedElementUpdated = null;

            if (baseElement is RootElement)
            {
                return;
            }

            foreach (var field in GetAllFields(baseElement.GetType()).DistinctBy(x => x.Name))
            {
                VisualElement visualElement = GetInspectorElement(field, baseElement);
                if (visualElement == null)
                    continue;

                visualElement.style.marginTop = ELEMENT_PADDING / 2;
                visualElement.style.marginBottom = ELEMENT_PADDING / 2;
                Add(visualElement);
            }
        }

        VisualElement GetInspectorElement(FieldInfo fieldInfo, object target)
        {
            switch (fieldInfo.FieldType)
            {
                // Common Types
                case var type when type == typeof(string): return RegisterStringInputField(fieldInfo, target);
                case var type when type == typeof(int): return RegisterIntInputField(fieldInfo, target);
                case var type when type == typeof(float): return RegisterFloatInputField(fieldInfo, target);
                case var type when type == typeof(Vector2): return Vector2fFieldElement(fieldInfo, target);
                case var type when type == typeof(Vector2Int): return Vector2iFieldElement(fieldInfo, target);
                case var type when type.IsEnum: return EnumFieldElement(fieldInfo, target);
                case var type when type == typeof(Color): return ColorFieldElement(fieldInfo, target);

                // Unity types
                case var type when type == typeof(UnityEngine.Texture2D): return ObjectFieldElement<UnityEngine.Texture2D>(fieldInfo, target);

                // Compound Types
                case var type when type == typeof(Shared.ScreenData.Types.Transform): return TransformInputElement.RegisterField(fieldInfo, target, this, editorState, ref onSelectedElementUpdated);

                default:
                    Debug.LogWarning($"Could not create inspector element for field type {fieldInfo.FieldType}");
                    return null;
            }
        }

        IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null)
                yield break;

            foreach (var fieldInfo in GetAllFields(type.BaseType))
            {
                yield return fieldInfo;
            }

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

        void AddOnSelectedElementUpdated(Action action)
        {
            if (onSelectedElementUpdated == null)
            {
                onSelectedElementUpdated = action;
            }
            else
            {
                onSelectedElementUpdated += action;
            }
        }

        VisualElement RegisterStringInputField(FieldInfo info, object target)
        {
            TextInputFieldElement textField = new TextInputFieldElement(info.Name, "");
            textField.SetValueWithoutNotify(info.GetValue(target)?.ToString() ?? "");
            textField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => textField.SetValueWithoutNotify(info.GetValue(target)?.ToString() ?? ""));
            return textField;
        }

        VisualElement RegisterIntInputField(FieldInfo info, object target)
        {
            IntInputFieldElement intField = new IntInputFieldElement(info.Name, 0);
            intField.SetValueWithoutNotify((int)info.GetValue(target));
            intField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => intField.SetValueWithoutNotify((int)info.GetValue(target)));
            return intField;
        }

        VisualElement RegisterFloatInputField(FieldInfo info, object target)
        {
            FloatInputFieldElement floatField = new FloatInputFieldElement(info.Name, 0f);
            floatField.SetValueWithoutNotify((float)info.GetValue(target));
            floatField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => floatField.SetValueWithoutNotify((float)info.GetValue(target)));
            return floatField;
        }

        VisualElement Vector2fFieldElement(FieldInfo info, object target)
        {
            Vector2fFieldElement vectorField = new Vector2fFieldElement(info.Name);
            vectorField.SetValueWithoutNotify((Vector2)info.GetValue(target));
            vectorField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => vectorField.SetValueWithoutNotify((Vector2)info.GetValue(target)));
            return vectorField;
        }

        VisualElement Vector2iFieldElement(FieldInfo info, object target)
        {
            Vector2iFieldElement vectorField = new Vector2iFieldElement(info.Name);
            vectorField.SetValueWithoutNotify((Vector2Int)info.GetValue(target));
            vectorField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => vectorField.SetValueWithoutNotify((Vector2Int)info.GetValue(target)));
            return vectorField;
        }

        VisualElement EnumFieldElement(FieldInfo info, object target)
        {
            EnumFieldElement enumField = new EnumFieldElement(info.Name, (Enum)info.GetValue(target));
            enumField.SetValueWithoutNotify((Enum)info.GetValue(target));
            enumField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => enumField.SetValueWithoutNotify((Enum)info.GetValue(target)));
            return enumField;
        }

        VisualElement ColorFieldElement(FieldInfo info, object target)
        {
            ColorFieldElement colorField = new ColorFieldElement(info.Name, (Color)info.GetValue(target));
            colorField.SetValueWithoutNotify((Color)info.GetValue(target));
            colorField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => colorField.SetValueWithoutNotify((Color)info.GetValue(target)));
            return colorField;
        }

        VisualElement ObjectFieldElement<T>(FieldInfo info, object target) where T : UnityEngine.Object
        {
            ObjectFieldElement<T> objectField = new ObjectFieldElement<T>(info.Name);
            objectField.SetValueWithoutNotify((T)info.GetValue(target));
            objectField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                editorState.TriggerElementIsDirty(this, new ValuesUpdated(editorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => objectField.SetValueWithoutNotify((T)info.GetValue(target)));
            return objectField;
        }
    }
}
