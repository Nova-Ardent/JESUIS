using System.Collections.Generic;
using UnityEngine;
using JESUIS.Runtime.Utilities;
using JESUIS.Shared.ScreenData.Data;

namespace JESUIS.Runtime.Screen.Layout
{
    public class LayoutBuilder
    {
        RootLayout rootLayout;
        ObjectPool<BaseLayout> baseLayouts;
        ObjectPool<BaseLayout> emptyLayouts;
        ObjectPool<BaseLayout> textureLayout;

        GameObject screenContainer;
        GameObject objectPoolContainer;

        LayoutLoader layoutLoader = new LayoutLoader();
        List<BaseLayout> activeElements = new List<BaseLayout>();

        public void Initialize(GameObject screenContainer)
        {
            this.screenContainer = screenContainer;

            objectPoolContainer = new GameObject("ObjectPoolContainer");
            objectPoolContainer.transform.SetParent(screenContainer.transform);

            layoutLoader.BuildLayoutLookup();

            baseLayouts = new ObjectPool<BaseLayout>(Resources.Load<BaseLayout>("ScreenData\\Data\\BaseLayout"), 0, objectPoolContainer);
            emptyLayouts = new ObjectPool<BaseLayout>(Resources.Load<EmptyLayout>("ScreenData\\Data\\EmptyLayout"), 0, objectPoolContainer);
            textureLayout = new ObjectPool<BaseLayout>(Resources.Load<TextureLayout>("ScreenData\\Data\\TextureLayout"), 0, objectPoolContainer);

            if (!screenContainer.TryGetComponent<RootLayout>(out rootLayout))
            {
                rootLayout = screenContainer.AddComponent<RootLayout>();
            }
        }

        public void ClearLayout()
        {
            foreach (BaseLayout layout in activeElements)
            {
                layout.ReleaseToPool();
            }
        }

        public void BuildLayout(System.Guid uid)
        {
            Shared.ScreenData.Screen screen = layoutLoader.LoadLayout(uid);
            if (screen == null)
            {
                throw new System.Exception($"Failed to load screen with uid {uid}, check that the metadata exists and is in the save path as the target screen, and that both are in a Resources folder");
            }

            rootLayout.SetLayout(screen.GetRootElement());
            RecursivelyBuildLayout(rootLayout, screen.GetRootElement());
        }

        void RecursivelyBuildLayout(BaseLayout baseLayout, BaseElement baseElement)
        {
            foreach (var child in baseElement.GetChildren())
            {
                BaseLayout childLayout = null;
                switch (child.GetType())
                {
                    default:
                        Debug.LogError($"undefined layout type, for type {baseElement.GetType()}");
                        break;
                    case var type when type == typeof(BaseElement):
                        childLayout = baseLayouts.Instantiate();
                        break;
                    case var type when type == typeof(EmptyElement):
                        childLayout = emptyLayouts.Instantiate();
                        break;
                    case var type when type == typeof(TextureElement):
                        childLayout = textureLayout.Instantiate();
                        break;
                }

                if (childLayout != null)
                {
                    activeElements.Add(childLayout);
                    childLayout.transform.SetParent(baseLayout.transform);
                    childLayout.SetLayout(child);
                    RecursivelyBuildLayout(childLayout, child);
                }
            }
        }
    }
}
