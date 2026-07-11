using System.Runtime.CompilerServices;

namespace JESUIS.Editor.Utilities.System.PathUtils
{
    public static partial class PathUtils
    {
        public static string GetCallerFilePath([CallerFilePath] string filePath = "")
        {
            return filePath;
        }
    }
}