using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine;

namespace JESUIS.Editor.Resources
{
    public class ResourceLoader
    {
        static ResourceLoader instance;
        public static ResourceLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourceLoader();
                }
                return instance;
            }
        }

        string basePath;

        public IconResources Icons;
        public ShaderResources Shaders;

        public class ShaderResources
        {
            public Resource<Shader> Background;

            public ShaderResources(string path)
            {
                Background = new Resource<Shader>(path, "Renderer/Background.shader");  
            }
        }

        public class IconResources
        {
            public HierarchyResources Hierarchy;
            public InspectorResources Inspector;
            public RendererResources Renderer;

            public IconResources(string path)
            {
                Hierarchy = new HierarchyResources(Path.Combine(path, "Hierarchy"));
                Inspector = new InspectorResources(Path.Combine(path, "Inspector"));
                Renderer = new RendererResources(Path.Combine(path, "Renderer"));
            }
        }

        public class HierarchyResources
        {
            public Resource<Texture2D> Root;
            public Resource<Texture2D> Image;

            public HierarchyResources(string path)
            {
                Root = new Resource<Texture2D>(path, "Root.png");
                Image = new Resource<Texture2D>(path, "Image.png");
            }
        }

        public class InspectorResources
        {
            public Resource<Texture2D> Transform;

            public InspectorResources(string path)
            {
                Transform = new Resource<Texture2D>(path, "Transform.png");
            }
        }

        public class RendererResources
        {
            public Resource<Texture2D> DragPoint;
            public Resource<Texture2D> DragArrow;
            public Texture2D EmptyCursor;

            public RendererResources(string path)
            {
                DragPoint = new Resource<Texture2D>(path, "DragIcon.png");
                DragArrow = new Resource<Texture2D>(path, "DragArrow.png");

                EmptyCursor = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                EmptyCursor.SetPixel(0, 0, new Color(0, 0, 0, 0));
                EmptyCursor.Apply();
            }
        }

        public ResourceLoader([CallerFilePath]string path = null)
        {
            string dataDir = Path.GetDirectoryName(Application.dataPath);
            string callerDir = Path.GetDirectoryName(path);
            basePath = Path.GetRelativePath(dataDir, callerDir);

            Icons = new IconResources(Path.Combine(basePath, "Icons"));
            Shaders = new ShaderResources(Path.Combine(basePath, "Shaders"));
        }
    }
}
