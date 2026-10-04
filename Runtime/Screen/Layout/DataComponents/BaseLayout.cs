using JESUIS.Runtime.Screen.Data;
using JESUIS.Runtime.Utilities;
using JESUIS.Shared.ScreenData.Data;
using JESUIS.Shared.ScreenData.DataBindings;
using JESUIS.Shared.ScreenData.Types;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace JESUIS.Runtime.Screen.Layout
{
    public class BaseLayout : MonoBehaviour, IPoolable<BaseLayout>
    {
        class ModelTransform
        {
            public ModelDataWrapperListener<Vector2> Size;
            public ModelDataWrapperListener<Vector2> Position;
            public ModelDataWrapperListener<Vector2> Scale;
            public ModelDataWrapperListener<float> Rotation;
            public ModelDataWrapperListener<Alignment> Anchor;
            public ModelDataWrapperListener<Alignment> Pivot;
            public ModelDataWrapperListener<Unit> VerticalPosition;
            public ModelDataWrapperListener<Unit> VerticalSize;
            public ModelDataWrapperListener<Unit> HorizontalPosition;
            public ModelDataWrapperListener<Unit> HorizontalSize;
        }

        private Vector2 lastKnownSize;
        protected BaseElement BaseElement;
        [SerializeField] protected RectTransform Transform;

        bool modelWasSynced = false;
        int lastModelSync = -1;
        ModelTransform modelTransform = null;
        
        protected Model Model;
        protected List<ModelDataWrapper> modelDataWrappers = new List<ModelDataWrapper>();
        protected List<BaseLayout> childrenLayouts = new List<BaseLayout>();

        public ObjectPool<BaseLayout> owningPool { get; set; }

        public void SetLayoutAndModel(BaseElement baseElement, Model model)
        {
            SetLayout(baseElement);
            SetModel(model);

            OnLayoutAndModelSet();
        }

        public void AddChildLayout(BaseLayout layout)
        {
            childrenLayouts.Add(layout);
        }

        protected virtual void OnLayoutAndModelSet()
        {
            modelTransform = new ModelTransform();
            SetupWrapperListener(ref modelTransform.Size, BaseElement.Transform.Size);
            SetupWrapperListener(ref modelTransform.Position, BaseElement.Transform.Position);
            SetupWrapperListener(ref modelTransform.Scale, BaseElement.Transform.Scale);
            SetupWrapperListener(ref modelTransform.Rotation, BaseElement.Transform.Rotation);
            SetupWrapperListener(ref modelTransform.Anchor, BaseElement.Transform.Anchor);
            SetupWrapperListener(ref modelTransform.Pivot, BaseElement.Transform.Pivot);
            SetupWrapperListener(ref modelTransform.VerticalPosition, BaseElement.Transform.VerticalPosition);
            SetupWrapperListener(ref modelTransform.VerticalSize, BaseElement.Transform.VerticalSize);
            SetupWrapperListener(ref modelTransform.HorizontalPosition, BaseElement.Transform.HorizontalPosition);
            SetupWrapperListener(ref modelTransform.HorizontalSize, BaseElement.Transform.HorizontalSize);
            PostModelSync();
        }

        void SetLayout(BaseElement baseElement)
        {
            this.BaseElement = baseElement;

#if UNITY_EDITOR
            UpdateName();
#endif
        }

        void SetModel(Model model)
        {
            this.Model = model;

#if UNITY_EDITOR
            UpdateName();
#endif
        }

        protected void SetupWrapperListener<T>(ref ModelDataWrapperListener<T> modelDataWrapper, Bindable<T> bindable)
        {
            modelDataWrapper = new ModelDataWrapperListener<T>(bindable.Value);

            if (!bindable.HasUID)
            {
                return;
            }

            ModelDataWrapper<T> dataWrapper = Model.GetDataWrapper<T>(bindable.Guid);
            dataWrapper.AddListener(this, modelDataWrapper);
            modelDataWrappers.Add(dataWrapper);
        }
        protected virtual void PostModelSync()
        {
            UpdateTransform();
        }

        public void ClearData()
        {
            foreach (var dataWrapper in modelDataWrappers)
            {
                dataWrapper.RemoveListener(this);
            }
            modelDataWrappers.Clear();

            this.modelTransform = null;
            this.lastModelSync = -1;
            this.Model = null;
            this.BaseElement = null;
            this.name = "";
            this.childrenLayouts.Clear();
        }

        public virtual void ReleaseToPool()
        {
            ClearData();
            owningPool.Release(this);
        }

        public void UpdateTransform()
        {
            if (modelTransform == null)
            {
                return;
            }

            Vector2 pivot = Vector2.zero;
            if (modelTransform.Pivot.Value.IsMiddleCol())
                pivot = new Vector2(0.5f, 0);
            else if (modelTransform.Pivot.Value.IsRight())
                pivot = new Vector2(1, 0);

            if (modelTransform.Pivot.Value.IsMiddleRow())
                pivot += new Vector2(0, 0.5f);
            else if (modelTransform.Pivot.Value.IsBottom())
                pivot += new Vector2(0, 1.0f);

            Vector2 anchor = Vector2.zero;
            if (modelTransform.Anchor.Value.IsMiddleCol())
                anchor = new Vector2(0.5f, 0);
            else if (modelTransform.Anchor.Value.IsRight())
                anchor = new Vector2(1, 0);

            if (modelTransform.Anchor.Value.IsMiddleRow())
                anchor += new Vector2(0, 0.5f);
            else if (modelTransform.Anchor.Value.IsBottom())
                anchor += new Vector2(0, 1.0f);

            Transform.pivot = new Vector2(pivot.x, 1 - pivot.y);
            Transform.anchorMin = new Vector2(anchor.x, 1 - anchor.y);
            Transform.anchorMax = new Vector2(anchor.x, 1 - anchor.y);

            RectTransform parent = (RectTransform)Transform.parent;
            Vector2 size = modelTransform.Size.Value;

            if (modelTransform.HorizontalSize.Value == Unit.Percentage && parent != null)
                size.x *= parent.sizeDelta.x / 100f;
            if (modelTransform.VerticalSize.Value == Unit.Percentage && parent != null)
                size.y *= parent.sizeDelta.y / 100f;

            Transform.sizeDelta = size;

            Vector2 localPosition = new Vector2(modelTransform.Position.Value.x, -modelTransform.Position.Value.y);
            if (modelTransform.HorizontalPosition.Value == Unit.Percentage && parent != null)
                localPosition.x *= parent.sizeDelta.x / 100f;
            if (modelTransform.VerticalPosition.Value == Unit.Percentage && parent != null)
                localPosition.y *= parent.sizeDelta.y / 100f;

            Transform.anchoredPosition = localPosition;

            Transform.localScale = modelTransform.Scale.Value;
            Transform.localRotation = Quaternion.Euler(0, 0, -modelTransform.Rotation.Value);
        }

        public void RecursivelySyncModels()
        {
            if (Model == null)
            {
                return;
            }

            if (lastModelSync != Model.LastUpdate())
            {
                lastModelSync = Model.LastUpdate();
                modelWasSynced = true;
            }

            foreach (BaseLayout layout in childrenLayouts)
            {
                layout.RecursivelySyncModels();
            }
        }

        public void UpdateChildren(bool parentWasUpdated, bool isRoot = false)
        {
            if (!isRoot)
            {
                if (parentWasUpdated || modelWasSynced)
                {
                    PostModelSync();
                    modelWasSynced = false;
                }
            }

            foreach (BaseLayout layout in childrenLayouts)
            {
                layout.UpdateChildren(!Mathf.Approximately(lastKnownSize.x, Transform.sizeDelta.x) || !Mathf.Approximately(lastKnownSize.y, Transform.sizeDelta.y));
                lastKnownSize = Transform.sizeDelta;
            }
        }

#if UNITY_EDITOR
        protected void UpdateName()
        {
            if (Model != null && BaseElement != null)
            {
                this.name = $"{Model.Name} - {BaseElement.Name}";
                return;
            }

            if (BaseElement != null)
            {
                this.name = $"{BaseElement.Name}";
                return;
            }

            this.name = this.GetType().Name;
        }
#endif
    }
}