using JESUIS.Editor.Elements.Interfaces;
using JESUIS.Editor.Helpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Window
{
    public class BaseWindow<T> : EditorWindow where T : EditorWindow
    {
        public BaseWindow() 
        {
        }

        protected static void LaunchWindow(string name)
        {
            var window = GetWindow<T>();
            window.titleContent = new UnityEngine.GUIContent(name);
            window.Show();
        }

        protected virtual IEnumerable<NamedAction> GetContextMenuOptions()
        {
            yield break;
        }

        protected virtual void CreateGUI()
        {
            rootVisualElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            rootVisualElement.style.backgroundColor = Settings.Colors.WINDOW_COLOR;
        }

        private void OnGUI()
        {
            Event evt = Event.current;

            if (evt.type == EventType.ContextClick)
            {
                ShowOptions();
                evt.Use();
            }
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            foreach (var child in rootVisualElement.Children())
            {
                if (child is IResizable basePanel)
                {
                    basePanel.Resize(evt.newRect.width, evt.newRect.height);
                }
            }
        }

        private void ShowOptions()
        {
            ContextMenuBuilder.BuildMenu(GetContextMenuOptions());
        }
    }
}
