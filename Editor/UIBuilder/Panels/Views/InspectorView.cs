using JESUIS.Editor.Elements.CompoundInputs;
using JESUIS.Editor.Elements.Input.Bindable;
using JESUIS.Editor.Elements.Input;
using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Resources;
using JESUIS.Editor.UIBuilder.Data.StateChanges;
using JESUIS.Editor.UIBuilder.Data;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData.DataBindings;
using JESUIS.Shared.ScreenData.Types;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine.UIElements;
using UnityEngine;

using static JESUIS.Shared.ScreenData.Data.PrefabElement;
using static JESUIS.Shared.ScreenData.Data.TextureElement;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class InspectorView : EditorViews
    {
        const int ELEMENT_PADDING = 2;

        Attribute[] attributes;
        Action onSelectedElementUpdated;

        public override Views Type => Views.Inspector;

        public InspectorView(EditorState editorState) : base(editorState)
        {
            style.left = 0;
            style.top = 0;
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
        }

        protected override void OnElementIsDirty(EditorViews triggeringView, ElementChanges elementChanges)
        {
            if (triggeringView.Type == Views.Inspector)
            {
                return;
            }

            if (elementChanges.ChangeType == ElementChanges.ElementChangeType.ValueUpdated && elementChanges.TargetElement == CurrentEditorState.SelectedElement.Value)
            {
                onSelectedElementUpdated?.Invoke();
            }
        }

        protected override void OnSelectedElementChanged(BaseElement baseElement)
        {
            Clear();
            onSelectedElementUpdated = null;

            if (baseElement == null)
            {
                return;
            }

            attributes = Attribute.GetCustomAttributes(baseElement.GetType());
            SetFieldsOfTarget(this, baseElement.GetType(), baseElement);
        }

        protected override void OnCurrentScreenChanged(Shared.ScreenData.Screen currentScreen)
        {
            OnSelectedElementChanged(null);
        }

        void SetFieldsOfTarget(VisualElement targetElement, Type targetType, object target)
        {
            if (target == null)
            {
                Debug.LogError($"target for type {targetType} is null");
                return;
            }

            foreach (var field in GetAllFields(targetType).DistinctBy(x => x.Name))
            {
                VisualElement visualElement = GetInspectorElement(field, targetType, target);
                if (visualElement == null)
                    continue;

                if (visualElement is not Container)
                {
                    visualElement.style.marginTop = ELEMENT_PADDING / 2;
                    visualElement.style.marginBottom = ELEMENT_PADDING / 2;
                }

                targetElement.Add(visualElement);
            }
        }

        VisualElement GetInspectorElement(FieldInfo fieldInfo, Type targetType, object target)
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
                case var type when type == typeof(Shared.ScreenData.Screen): return ObjectFieldElement<Shared.ScreenData.Screen>(fieldInfo, target);
                case var type when type == typeof(UnityEngine.Texture2D): return ObjectFieldElement<UnityEngine.Texture2D>(fieldInfo, target);

                // Compound Types
                case var type when type == typeof(Shared.ScreenData.Types.Transform):
                    if (targetType == typeof(RootElement))
                    {
                        return RootTransformFieldElement(fieldInfo, target);
                    }
                    else
                    {
                        return TransformInputElement.RegisterField(fieldInfo, target, this, CurrentEditorState, ref onSelectedElementUpdated, attributes.Where(x => x is TransformElementLock).Cast<TransformElementLock>().ToArray());
                    }

                // Bindable Types
                case var type when type == typeof(Bindable<string>): return RegisterStringBindableInputField(fieldInfo, target);
                case var type when type == typeof(Bindable<int>): return RegisterIntBindableInputField(fieldInfo, target);
                case var type when type == typeof(Bindable<float>): return RegisterFloatBindableInputField(fieldInfo, target);
                case var type when type == typeof(Bindable<Vector2>): return Vector2fBindableFieldElement(fieldInfo, target);
                case var type when type == typeof(Bindable<Vector2Int>): return Vector2iBindableFieldElement(fieldInfo, target);
                case var type when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Bindable<>) && type.GetGenericArguments()[0].IsEnum: 
                    return EnumBindableFieldElement(fieldInfo, target);
                case var type when type == typeof(Bindable<Color>): return ColorBindableFieldElement(fieldInfo, target);

                // Unity Bindable Types
                case var type when type == typeof(Bindable<Shared.ScreenData.Screen>): return ObjectBindableFieldElement<Shared.ScreenData.Screen>(fieldInfo, target);
                case var type when type == typeof(Bindable<UnityEngine.Texture2D>): return ObjectBindableFieldElement<UnityEngine.Texture2D>(fieldInfo, target);

                case var type when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Bindable<>): 
                    Debug.LogError($"found bindable type {type}, but not sure how to handle it. Please implement handling for this type.");
                    return null;
                    
                default:
                    if (fieldInfo.FieldType.IsDefined(typeof(System.SerializableAttribute), true))
                    {
                        Container container = new Container(fieldInfo.FieldType.Name, fieldInfo.Name, GetTextureForFieldType(fieldInfo.FieldType));
                        SetFieldsOfTarget(container, fieldInfo.FieldType, fieldInfo.GetValue(target));
                        return container;
                    }

                    Debug.LogWarning($"Could not create inspector element for field type {fieldInfo.FieldType}");
                    return null;
            }
        }

        Texture2D GetTextureForFieldType(Type type)
        {
            return type switch
            {
                var t when t == typeof(PrefabData) => ResourceLoader.Instance.Icons.Hierarchy.Prefab.Value,
                var t when t == typeof(ImageData) => ResourceLoader.Instance.Icons.Hierarchy.Image.Value,
                _ => null
            };
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

        // COMMON INPUT FIELDS
        VisualElement RootTransformFieldElement(FieldInfo info, object target)
        {
            Shared.ScreenData.Types.Transform transform = (Shared.ScreenData.Types.Transform)info.GetValue(target);

            BindableVector2fFieldElement vectorField = new BindableVector2fFieldElement("Size", "W", "H");
            vectorField.Set(transform.Size);
            vectorField.RegisterOnValueChanged(newValue =>
            {
                transform.Size = newValue;
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => vectorField.Set(transform.Size));
            return vectorField;
        }

        VisualElement RegisterStringInputField(FieldInfo info, object target)
        {
            TextInputFieldElement textField = new TextInputFieldElement(info.Name, "");
            textField.SetValueWithoutNotify(info.GetValue(target)?.ToString() ?? "");
            textField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
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
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => objectField.SetValueWithoutNotify((T)info.GetValue(target)));
            return objectField;
        }
        // COMMON INPUT FIELDS

        // BINDABLE INPUT FIELDS
        VisualElement RegisterStringBindableInputField(FieldInfo info, object target)
        {
            BindableTextInputFieldElement textField = new BindableTextInputFieldElement(info.Name, "");

            textField.SetTextBindingWithoutNotify((Bindable<string>)info.GetValue(target));
            textField.RegisterOnValueChanged(newBindable =>
            {
                info.SetValue(target, newBindable);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() =>
            {
                textField.SetTextBindingWithoutNotify((Bindable<string>)info.GetValue(target));
            });
            return textField; 
        }

        VisualElement RegisterIntBindableInputField(FieldInfo info, object target)
        {
            BindableIntInputFieldElement intField = new BindableIntInputFieldElement(info.Name, 0);
            intField.SetWithoutNotify((Bindable<int>)info.GetValue(target));
            intField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => intField.SetWithoutNotify((Bindable<int>)info.GetValue(target)));
            return intField;
        }

        VisualElement RegisterFloatBindableInputField(FieldInfo info, object target)
        {
            BindableFloatInputFieldElement intField = new BindableFloatInputFieldElement(info.Name, 0);
            intField.SetWithoutNotify((Bindable<float>)info.GetValue(target));
            intField.RegisterOnValueChanged(newText =>
            {
                info.SetValue(target, newText);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => intField.SetWithoutNotify((Bindable<float>)info.GetValue(target)));
            return intField;
        }

        VisualElement Vector2fBindableFieldElement(FieldInfo info, object target)
        {
            BindableVector2fFieldElement vectorField = new BindableVector2fFieldElement(info.Name);
            vectorField.Set((Bindable<Vector2>)info.GetValue(target));
            vectorField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => vectorField.Set((Bindable<Vector2>)info.GetValue(target)));
            return vectorField;
        }

        VisualElement Vector2iBindableFieldElement(FieldInfo info, object target)
        {
            BindableVector2iFieldElement vectorField = new BindableVector2iFieldElement(info.Name);
            vectorField.Set((Bindable<Vector2Int>)info.GetValue(target));
            vectorField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => vectorField.Set((Bindable<Vector2Int>)info.GetValue(target)));
            return vectorField;
        }

        VisualElement EnumBindableFieldElement(FieldInfo info, object target)
        {
            Shared.ScreenData.DataBindings.IBindable bindable = (Shared.ScreenData.DataBindings.IBindable)info.GetValue(target);
            BindableEnumFieldElement<Enum> enumField = new BindableEnumFieldElement<Enum>(info.Name, (Enum)bindable.GetBindingValue(), bindable.GetBindingValue().GetType());
            enumField.SetValueWithoutNotify((Enum)bindable.GetBindingValue());
            enumField.SetBindingWithoutNotify(bindable.UIDHigh, bindable.UIDLow);

            enumField.RegisterOnValueChanged(newValue =>
            {
                bindable.SetBindingValue(newValue.Value);
                bindable.UIDHigh = newValue.UIDHigh;
                bindable.UIDLow = newValue.UIDLow;

                info.SetValue(target, bindable);
                
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() =>
            {
                Shared.ScreenData.DataBindings.IBindable updatedBindable = (Shared.ScreenData.DataBindings.IBindable)info.GetValue(target);
                enumField.SetValueWithoutNotify((Enum)updatedBindable.GetBindingValue());
                enumField.SetBindingWithoutNotify(updatedBindable.UIDHigh, updatedBindable.UIDLow);
            });
            return enumField;
        }

        VisualElement ColorBindableFieldElement(FieldInfo info, object target)
        {
            BindableColorFieldElement colorFieldElement = new BindableColorFieldElement(info.Name, Color.white);
            colorFieldElement.Set((Bindable<Color>)info.GetValue(target));
            colorFieldElement.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => colorFieldElement.Set((Bindable<Color>)info.GetValue(target)));
            return colorFieldElement;
        }

        VisualElement ObjectBindableFieldElement<T>(FieldInfo info, object target) where T : UnityEngine.Object
        {
            BindableObjectFieldElement<T> objectField = new BindableObjectFieldElement<T>(info.Name);
            objectField.Set((Bindable<T>)info.GetValue(target));
            objectField.RegisterOnValueChanged(newValue =>
            {
                info.SetValue(target, newValue);
                CurrentEditorState.TriggerElementIsDirty(this, new ValuesUpdated(CurrentEditorState.SelectedElement));
            });

            AddOnSelectedElementUpdated(() => objectField.Set((Bindable<T>)info.GetValue(target)));
            return objectField;
        }
        // BINDABLE INPUT FIELDS
    }
}
