using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

namespace JESUIS.Editor.Helpers
{
    public static class ContextMenuBuilder
    {
        public static void BuildMenu(params NamedAction[] actions)
        {
            BuildMenu(null, actions.AsEnumerable());
        }

        public static void BuildMenu(IEnumerable<NamedAction> actions)
        {
            BuildMenu(null, actions);
        }

        public static void BuildMenu(Action<int> onComplete, params NamedAction[] actions)
        {
            BuildMenu(null, onComplete, actions.AsEnumerable());
        }

        public static void BuildMenu(Action<int> onComplete, IEnumerable<NamedAction> actions)
        {
            BuildMenu(null, onComplete, actions);
        }

        public static void BuildMenu(Rect? rect, Action<int> onComplete, params NamedAction[] actions)
        {
            BuildMenu(rect, onComplete, actions.AsEnumerable());
        }

        public static void BuildMenu(Rect? rect, Action<int> onComplete, IEnumerable<NamedAction> actions)
        {
            GenericMenu genericMenu = new GenericMenu();

            int i = 0;
            foreach (var action in actions)
            {
                int result = i;
                genericMenu.AddItem(new GUIContent(action.Name), action.IsOn, () =>
                {
                    action.Action();
                    onComplete?.Invoke(result);
                });

                i++;
            }

            if (rect.HasValue)
            {
                genericMenu.DropDown(rect.Value);
            }
            else
            {
                genericMenu.ShowAsContext();
            }
        }
    }
}