using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Shared.ScreenData.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEngine;

public class RendererElementLoader
{
    static RendererElementLoader _instance;
    public static RendererElementLoader Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RendererElementLoader();
                _instance.Build();
            }
            return _instance;
        }
    }

    Dictionary<Type, Type> rendererElementTypes = new Dictionary<Type, Type>();

    public VisualElement InstantiateRendererElement<T>(T data) where T : BaseElement
    {
        Type type = data.GetType();
        Type resultType = GetRendererElementType(type);
        object resultValue = Activator.CreateInstance(resultType);

        if (resultValue is IRendererElement<T> rendererElement)
        {
            rendererElement.Data = data;
        }
        else
        {
            Debug.LogError($"{resultType} does not contain interface {typeof(IRendererElement<T>)}");
        }

        if (resultValue is VisualElement visualElement)
        {
            return visualElement;
        }

        Debug.LogError($"{resultType} does not extend visual element");
        return null;
    }

    public Type GetRendererElementType(Type targetType)
    {
        if (rendererElementTypes.ContainsKey(targetType))
            return rendererElementTypes[targetType];
        
        throw new Exception($"Target type {targetType} does not contain a designated renderer type");
    }

    public void Build()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!Attribute.IsDefined(type, typeof(RendererElementAttribute)))
                {
                    continue;
                }

                if (!type.IsClass)
                {
                    Debug.LogError($"Failed to add type {type} to the renderer element types, as it is not a class");
                    continue;
                }

                if (!type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRendererElement<>)))
                {
                    Debug.LogError($"Renderer element type {type} does not implement {typeof(IRendererElement<>)} type");
                    continue;
                }

                RendererElementAttribute rendererAttribute = (RendererElementAttribute)Attribute.GetCustomAttribute(type, typeof(RendererElementAttribute));
                Type rendererAttributeTarget = rendererAttribute.ElementType;

                List<Type> genericArguments = new List<Type>();
                foreach (var currentInterface in type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRendererElement<>)))
                {
                    genericArguments.AddRange(currentInterface.GetGenericArguments());
                }

                if (!genericArguments.Any(x => x == rendererAttributeTarget))
                {
                    Debug.LogError($"IRendererElement interface generic does not match attribute passed type for {type}");
                    continue;
                }

                if (!typeof(VisualElement).IsAssignableFrom(type))
                {
                    Debug.LogError($"{type} does not implement {typeof(VisualElement)}");
                    continue;
                }

                rendererElementTypes[rendererAttributeTarget] = type; 
            }
        }
    }
}
