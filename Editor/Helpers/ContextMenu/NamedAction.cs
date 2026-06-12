using System;

namespace JESUIS.Editor.Helpers
{
    public struct NamedAction
    {
        public NamedAction(string name, Action action, bool isOn)
        {
            Name = name;
            Action = action;
            IsOn = isOn;
        }

        public Action Action { get; }
        public string Name { get; }
        public bool IsOn { get; }
    }
}
