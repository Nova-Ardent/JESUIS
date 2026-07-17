using JESUIS.Shared.ScreenData.Data;
using System;
using UnityEngine;

namespace JESUIS.Editor.UIBuilder.Panels.Views.Renderer.Hierarchy.Builder
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RendererElementAttribute : Attribute
    {
        public Type ElementType { get; private set; }
        public RendererElementAttribute(Type elementType)
        {
            ElementType = elementType;
            if (!elementType.IsAssignableFrom(typeof(BaseElement)))
            {
                Debug.LogError($"Renderer has invalid type argument, needs to be subclass of {typeof(BaseElement)}");
            }
        }
    }
}