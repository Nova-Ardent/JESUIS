using JESUIS.Runtime.Utilities;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData.Types;
using System.Collections;
using UnityEngine;

namespace JESUIS.Runtime.Screen.Layout
{
    public class BaseLayout : MonoBehaviour, IPoolable<BaseLayout>
    {
        private Vector2 lastKnownSize;
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

        public void UpdateTransform()
        {
            Vector2 pivot = Vector2.zero;
            if (baseElement.Transform.Pivot.Value.IsMiddleCol())
                pivot = new Vector2(0.5f, 0);
            else if (baseElement.Transform.Pivot.Value.IsRight())
                pivot = new Vector2(1, 0);

            if (baseElement.Transform.Pivot.Value.IsMiddleRow())
                pivot += new Vector2(0, 0.5f);
            else if (baseElement.Transform.Pivot.Value.IsBottom())
                pivot += new Vector2(0, 1.0f);

            Vector2 anchor = Vector2.zero;
            if (baseElement.Transform.Anchor.Value.IsMiddleCol())
                anchor = new Vector2(0.5f, 0);
            else if (baseElement.Transform.Anchor.Value.IsRight())
                anchor = new Vector2(1, 0);

            if (baseElement.Transform.Anchor.Value.IsMiddleRow())
                anchor += new Vector2(0, 0.5f);
            else if (baseElement.Transform.Anchor.Value.IsBottom())
                anchor += new Vector2(0, 1.0f);

            Transform.pivot = new Vector2(pivot.x, 1 - pivot.y);
            Transform.anchorMin = new Vector2(anchor.x, 1 - anchor.y);
            Transform.anchorMax = new Vector2(anchor.x, 1 - anchor.y);

            RectTransform parent = (RectTransform)Transform.parent;
            Vector2 size = baseElement.Transform.Size.Value;

            if (baseElement.Transform.HorizontalSize.Value == Unit.Percentage && parent != null)
                size.x *= parent.sizeDelta.x / 100f;
            if (baseElement.Transform.VerticalSize.Value == Unit.Percentage && parent != null)
                size.y *= parent.sizeDelta.y / 100f;

            Transform.sizeDelta = size;

            Vector2 localPosition = new Vector2(baseElement.Transform.Position.Value.x, -baseElement.Transform.Position.Value.y);
            if (baseElement.Transform.HorizontalPosition.Value == Unit.Percentage && parent != null)
                localPosition.x *= parent.sizeDelta.x / 100f;
            if (baseElement.Transform.VerticalPosition.Value == Unit.Percentage && parent != null)
                localPosition.y *= parent.sizeDelta.y / 100f;

            Transform.anchoredPosition = localPosition;

            Transform.localScale = baseElement.Transform.Scale.Value;
            Transform.localRotation = Quaternion.Euler(0, 0, baseElement.Transform.Rotation.Value);
        }

        public void UpdateChildren()
        {
            if (Mathf.Approximately(lastKnownSize.x, Transform.sizeDelta.x) && Mathf.Approximately(lastKnownSize.y, Transform.sizeDelta.y))
            {
                return;
            }

            lastKnownSize = Transform.sizeDelta;
            foreach (UnityEngine.Transform child in Transform)
            {
                BaseLayout layout = child.GetComponent<BaseLayout>();
                if (layout != null)
                {
                    layout.UpdateTransform();
                    layout.UpdateChildren();
                }
            }
        }
    }
}