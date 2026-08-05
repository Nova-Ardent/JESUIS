using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.Utilities
{
    public static class Utilities
    {
        public static StyleSheet GetUSS(string styleSheetName = "style", [CallerFilePath] string callerPath = "")
        {
            string dataDir = Path.GetDirectoryName(Application.dataPath);
            string callerDir = Path.GetDirectoryName(callerPath);
            string relativePath = Path.GetRelativePath(dataDir, callerDir);

            string target = Path.Combine(relativePath, styleSheetName);
            StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(target);
            if (sheet == null)
            {
                Debug.LogError("Failed to load stylesheet at :" + target);
            }
            return sheet;
        }

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