using System.IO;
using UnityEngine;

namespace JESUIS.Editor.Utilities.System.PathUtils
{
    public static partial class PathUtils
    {
        /// <summary>
        /// Converts an absolute path into one relative to the project root, which is the form
        /// every AssetDatabase call expects. Separators are normalised to '/' so the result is
        /// usable as an asset path on every platform.
        /// </summary>
        public static string GetProjectRelativePath(string absolutePath)
        {
            string projectDirectory = Path.GetDirectoryName(Application.dataPath);
            return Path.GetRelativePath(projectDirectory, absolutePath).Replace('\\', '/');
        }
    }
}
