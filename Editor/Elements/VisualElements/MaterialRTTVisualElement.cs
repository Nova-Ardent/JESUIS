using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine;

namespace JESUIS.Editor.Elements.Common.VisualElements
{
    public class MaterialRTTVisualElement : VisualElement
    {
        ushort[] indices;
        Vertex[] vertices;

        Shader shader;
        Material material;
        RenderTexture renderTexture;

        public MaterialRTTVisualElement(Shader shader)
        {
            if (shader == null)
            {
                Debug.LogError("Shader is null");
                return;
            }

            material = new Material(shader);
            this.shader = shader;

            generateVisualContent += OnGenerateVisualContent;
            UpdateIndexArray();
        }

        ~MaterialRTTVisualElement()
        {
            renderTexture?.Release();
            UnityEngine.Object.DestroyImmediate(renderTexture);
            UnityEngine.Object.DestroyImmediate(material);
        }

        void UpdateIndexArray()
        {
            indices = new ushort[6] { 0, 1, 2, 2, 3, 0 };
        }

        void UpdateVertexArray(int width, int height)
        {
            vertices = new Vertex[4];

            float left = 0;
            float right = width;
            float top = 0;
            float bottom = height;

            vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
            vertices[1].position = new Vector3(left, top, Vertex.nearZ);
            vertices[2].position = new Vector3(right, top, Vertex.nearZ);
            vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

            vertices[0].uv = new Vector2(0, 1);
            vertices[1].uv = new Vector2(0, 0);
            vertices[2].uv = new Vector2(1, 0);
            vertices[3].uv = new Vector2(1, 1);

            vertices[0].tint = Color.white;
            vertices[1].tint = Color.white;
            vertices[2].tint = Color.white;
            vertices[3].tint = Color.white;
        } 

        public Material GetMaterial()
        {
            return material;
        }

        public void SetSize(int width, int height)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                UnityEngine.Object.DestroyImmediate(renderTexture); 
            }

            renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
            {
                name = $"RTT_{shader.name}",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };

            renderTexture.Create();

            style.width = width;
            style.height = height;

            UpdateVertexArray(width, height);
            UpdateTexture();
        }

        public void UpdateTexture()
        {
            schedule.Execute(() =>
            {
                if (material == null)
                {
                    material = new Material(shader);
                }

                Graphics.Blit(Texture2D.whiteTexture, renderTexture, material);
                MarkDirtyRepaint();
            });
        }

        void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            if (renderTexture == null)
            {
                return;
            }

            var nativeIndices = new NativeArray<ushort>(indices.Length, Allocator.Temp);
            nativeIndices.CopyFrom(indices);
            NativeSlice<ushort> nativeIndexSlice = new NativeSlice<ushort>(nativeIndices);

            var nativeVertices = new NativeArray<Vertex>(vertices.Length, Allocator.Temp);
            nativeVertices.CopyFrom(vertices);
            NativeSlice<Vertex> nativeVerticesSlice = new NativeSlice<Vertex>(nativeVertices);

            ctx.DrawMesh(nativeVerticesSlice, nativeIndexSlice, renderTexture);
        }
    }
}
