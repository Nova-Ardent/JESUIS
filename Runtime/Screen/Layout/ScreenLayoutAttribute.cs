using System;

namespace JESUIS.Runtime.Screen.Layout
{
    public class ScreenLayoutAttribute : Attribute
    {
        public Guid Guid { get; private set; }

        public ScreenLayoutAttribute(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new Exception("ScreenDataAttribute requires a non-empty uid");
            }

            if (Guid.TryParse(uid, out var guid))
            {
                Guid = guid;
            }
            else
            {
                throw new Exception("ScreenDataAttribute requires a valid GUID");
            }
        }
    }
}
