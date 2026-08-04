using UnityEngine;

namespace JESUIS.Shared.ScreenData.Data
{
    [System.Serializable]
    public class TextureElement : EmptyElement
    {
        [System.Serializable]
        public class ImageData
        {
            public Texture2D Texture;
            public Color Color;
        }

        [SerializeField] public ImageData Image = new ImageData();
    }
}