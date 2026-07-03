using JESUIS.Editor.Elements.Interfaces;
using JESUIS.Editor.Elements.Layout;
using JESUIS.Editor.Helpers;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Panel
{
    public class BasePanel : VisualElement
        , IResizable
    {
        public BasePanel()
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        public BasePanel(float width, float height) : this()
        {
            style.width = width;
            style.height = height;
        }

        public virtual void Resize(float width, float height)
        {
            style.width = width;
            style.height = height;
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button == 1)
            {
                ShowOptions();
                evt.StopImmediatePropagation();
            }
        }

        private void ShowOptions()
        {
            ContextMenuBuilder.BuildMenu(GetContextMenuOptions());
        }

        protected virtual IEnumerable<NamedAction> GetContextMenuOptions()
        {
            yield break;
        }
    }
}