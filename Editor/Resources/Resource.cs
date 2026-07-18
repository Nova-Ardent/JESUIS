using UnityEditor;
using System.IO;

namespace JESUIS.Editor.Resources
{
    public class Resource<T> where T : UnityEngine.Object
    {
        string path = "";

        public Resource(string path, string file)
        {
            this.path = Path.Combine(path, file);
        }

        T value;
        public T Value 
        {
            get
            {
                if (value == null)
                {
                    value = AssetDatabase.LoadAssetAtPath<T>(path);
                }
                return value;
            }
        }
    }
}
