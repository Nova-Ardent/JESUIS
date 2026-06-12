using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JESUIS.Editor.Helpers
{
    public static class ContextMenuBuilder
    {
        public static void BuildMenu(params NamedAction[] actions)
        {
            BuildMenu(actions.AsEnumerable());
        }

        public static void BuildMenu(IEnumerable<NamedAction> actions)
        {
            GenericMenu genericMenu = new GenericMenu();
            foreach (var action in actions)
            {
                genericMenu.AddItem(new GUIContent(action.Name), action.IsOn, () => action.Action());
            }
            genericMenu.ShowAsContext();
        }
    }
}