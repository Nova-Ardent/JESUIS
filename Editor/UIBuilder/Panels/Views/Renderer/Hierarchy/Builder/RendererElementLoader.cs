using JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder;
using JESUIS.Shared.ScreenData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    public VisualElement InstantiateRendererElement(BaseElement data)
    {
        Type resultType = GetRendererElementType(data.GetType());
        if (resultType == null)
        {
            return null;
        }

        object resultValue = Activator.CreateInstance(resultType);

        if (resultValue is IRendererElement rendererElement)
        {
            rendererElement.SetData(data);
        }
        else
        {
            Debug.LogError($"{resultType} does not contain interface {typeof(IRendererElement)}");
        }

        if (resultValue is VisualElement visualElement)
        {
            return visualElement;
        }

        Debug.LogError($"{resultType} does not extend visual element");
        return null;
    }

    /// <summary>
    /// Walks up the inheritance chain so an element type without its own renderer falls back to the
    /// nearest registered ancestor, ultimately <see cref="BaseElement"/>. The resolved renderer is
    /// registered against the type that was asked for, so the walk runs once per element type
    /// rather than once per element.
    /// </summary>
    public Type GetRendererElementType(Type targetType)
    {
        for (Type type = targetType; type != null; type = type.BaseType)
        {
            if (rendererElementTypes.TryGetValue(type, out Type rendererType))
            {
                rendererElementTypes[targetType] = rendererType;
                return rendererType;
            }
        }

        Debug.LogError($"Target type {targetType} does not contain a designated renderer type");
        return null;
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

                Type[] genericArguments = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRendererElement<>)).GetGenericArguments();
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
