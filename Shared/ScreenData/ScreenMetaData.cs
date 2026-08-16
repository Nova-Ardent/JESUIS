using System;
using System.IO;
using UnityEngine;

namespace JESUIS.Shared.ScreenData
{
    [System.Serializable]
    public class ScreenMetaData : ScriptableObject
    {
        [SerializeField] public string Uid;

        [SerializeField] string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = System.IO.Path.GetFileNameWithoutExtension(value);
            }
        }

        [SerializeField] string path;
        public string Path
        {
            get
            {
                return path;
            }
        }

        public void Initialize()
        {
            Uid = Guid.NewGuid().ToString("N");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Uid))
            {
                Uid = Guid.NewGuid().ToString("N");
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        public bool TryUpdatePath(string path)
        {
            if (path == this.path)
            {
                return false;
            }

            if (TryTrimToResourcesPath(path, out this.path))
            {
                return true;
            }

            Debug.LogError("path must be a valid resource path, for screen meta data");
            return false;
        }

        static bool TryTrimToResourcesPath(string path, out string resourcesPath)
        {
            const string resourcesFolder = "Resources";

            int index = path.IndexOf(
                resourcesFolder,
                StringComparison.OrdinalIgnoreCase);

            if (index < 0)
            {
                resourcesPath = null;
                return false;
            }

            resourcesPath = path.Substring(index + resourcesFolder.Length);
            if (!string.IsNullOrEmpty(resourcesPath))
            {
                if (resourcesPath[0] == '\\' || resourcesPath[0] == '/') 
                {
                    resourcesPath = resourcesPath.Substring(1);
                }
            }

            resourcesPath = System.IO.Path.ChangeExtension(resourcesPath, null);
            return true;
        }
#endif
    }
}
