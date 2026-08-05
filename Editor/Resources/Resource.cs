using System.IO;
using UnityEditor;

namespace JESUIS.Editor.Resources
{
    public class Resource<T> where T : UnityEngine.Object
    {
        protected string path = "";

        public Resource(string path, string file)
        {
            this.path = Path.Combine(path, file);
        }

        protected T value;
        public virtual T Value 
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
