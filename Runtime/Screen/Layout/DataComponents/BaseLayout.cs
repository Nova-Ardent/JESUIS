using JESUIS.Runtime.Utilities;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData.Types;
using System.Collections;
using UnityEngine;

namespace JESUIS.Runtime.Screen.Layout
{
    public class BaseLayout : MonoBehaviour, IPoolable<BaseLayout>
    {
        private BaseElement baseElement;
        [SerializeField] protected RectTransform Transform;

        public ObjectPool<BaseLayout> owningPool { get; set; }

        public virtual void SetLayout(BaseElement baseElement)
        {
            this.baseElement = baseElement;
            this.name = baseElement.Name;

            UpdateTransform();
        }

        public virtual void ReleaseToPool()
        {
            this.baseElement = null;
            this.name = "";
            owningPool.Release(this);
        }

        void UpdateTransform()
        {
            Vector2 pivot = Vector2.zero;
            if (baseElement.Transform.Pivot.IsMiddleCol())
                pivot = new Vector2(0.5f, 0);
            else if (baseElement.Transform.Pivot.IsRight())
                pivot = new Vector2(1, 0);

            if (baseElement.Transform.Pivot.IsMiddleRow())
                pivot += new Vector2(0, 0.5f);
            else if (baseElement.Transform.Pivot.IsBottom())
                pivot += new Vector2(0, 1.0f);

            Vector2 anchor = Vector2.zero;
            if (baseElement.Transform.Anchor.IsMiddleCol())
                anchor = new Vector2(0.5f, 0);
            else if (baseElement.Transform.Anchor.IsRight())
                anchor = new Vector2(1, 0);

            if (baseElement.Transform.Anchor.IsMiddleRow())
                anchor += new Vector2(0, 0.5f);
            else if (baseElement.Transform.Anchor.IsBottom())
                anchor += new Vector2(0, 1.0f);

            Transform.pivot = new Vector2(pivot.x, 1 - pivot.y);
            Transform.anchorMin = new Vector2(anchor.x, 1 - anchor.y);
            Transform.anchorMax = new Vector2(anchor.x, 1 - anchor.y);

            Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseElement.Transform.Size.x);
            Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseElement.Transform.Size.y);

            Transform.localScale = baseElement.Transform.Scale;
            Transform.anchoredPosition = baseElement.Transform.Position;
            Transform.localRotation = Quaternion.Euler(0, 0, baseElement.Transform.Rotation);
        }
    }
}