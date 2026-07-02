namespace JESUIS.Shared.ScreenData.Data
{
    [System.Serializable]
    public class RootElement : BaseElement
    {
        public const string DefaultRootName = "Root Element";

        public RootElement()
        {
            Name = DefaultRootName;
        }
    }
}