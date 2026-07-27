using UnityEditor;
using UnityEngine;

namespace JESUIS.Editor.Resources
{
    public class CursorTextureResource : Resource<Texture2D>
    {
        public CursorTextureResource(string path, string file) : base(path, file)
        {
        }

        public override Texture2D Value
        {
            get
            {
                if (value == null)
                {
                    var temp = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (!temp.isReadable)
                    {
                        Debug.LogError("target texture, " + path);
                        Object.DestroyImmediate(temp);
                        return null;
                    }

                    Texture2D cursorTex = new Texture2D(
                        temp.width,
                        temp.height,
                        TextureFormat.RGBA32,
                        false
                    );

                    cursorTex.SetPixels32(temp.GetPixels32());
                    cursorTex.Apply(false, false);
                    
                    value = cursorTex;
                }
                return value;
            }
        }
    }
}
