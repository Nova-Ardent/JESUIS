using System;
using System.Collections.Generic;
using System.Linq;

namespace JESUIS.Editor.Utilities
{
    public static class Utilities
    {
        public static IEnumerable<Enum> GetEnums(Type type)
        {
            foreach (var e in Enum.GetValues(type))
            {
                Enum ret = e as Enum;
                if (ret == null)
                    continue;

                yield return ret;
            }
        }

        public static IEnumerable<T> GetEnums<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}