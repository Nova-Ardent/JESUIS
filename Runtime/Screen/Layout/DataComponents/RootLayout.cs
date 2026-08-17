using JESUIS.Shared.ScreenData.Data;
using UnityEngine;

namespace JESUIS.Runtime.Screen.Layout
{
    public class RootLayout : BaseLayout
    {
        private void Awake()
        {
            TryGetComponent<RectTransform>(out Transform);
        }

        public override void SetLayout(BaseElement baseElement)
        {
        }
    }
}