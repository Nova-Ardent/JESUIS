using System;

namespace JESUIS.Editor.Utilities.System.PathUtils
{
    public static partial class PathUtils
    {
        public static bool TryTrimToResourcesPath(string path, out string resourcesPath)
        {
            const string folder = "/Resources/";

            int index = path.IndexOf(folder, StringComparison.OrdinalIgnoreCase);

            if (index < 0)
            {
                resourcesPath = null;
                return false;
            }

            resourcesPath = path.Substring(index + folder.Length);
            return true;
        }
    }
}