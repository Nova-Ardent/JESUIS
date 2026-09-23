using JESUIS.Shared.ScreenData.DataBindings;
using JESUIS.Shared.ScreenData.Types;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.Data
{
    [System.Serializable]
    public class TextureElement : EmptyElement
    {
        [System.Serializable]
        public class ImageData
        { 
            public Bindable<Texture2D> Texture;
            public Bindable<Color> Color = new Bindable<Color>() { Value = UnityEngine.Color.white };
        }

        [SerializeField] public ImageData Image = new ImageData();
    }
}