using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Utilities.StyleSheets
{
    public class USSInstanceLoader
    {
        public StyleSheet StyleSheet;
    }

    public class USSInstanceLoader<T> : USSInstanceLoader where T : USSInstanceLoader, new()
    {
        static USSInstanceLoader loaderInstance;
        public static USSInstanceLoader LoaderInstance
        {
            get
            {
                if (loaderInstance == null)
                {
                    loaderInstance = new T();
                }
                return loaderInstance;
            }
        }

        public static StyleSheet StyleSheetInstance
        {
            get { return LoaderInstance.StyleSheet; } 
        }

        public USSInstanceLoader(string fileName, [CallerFilePath] string path = null)
        {
            StyleSheet = Utilities.GetUSS(fileName, path);
        }
    }
}
