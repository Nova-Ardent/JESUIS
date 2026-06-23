using System;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class ViewManager
    {
        Shared.ScreenData.Screen screenData;

        Action<EditorViews.Views> onViewChanged;

        public EditorViews NoneView { get => new EditorViews(); }
        public HierarchyView CurrentHierarchyView { get; private set; }

        public ViewManager(Shared.ScreenData.Screen screenData)
        {
            this.screenData = screenData;

            CurrentHierarchyView = new HierarchyView(screenData);
        }

        public EditorViews GetView(EditorViews.Views view, bool triggerOnChange = true)
        {
            switch (view)
            {
                case EditorViews.Views.Hierarchy:
                    onViewChanged?.Invoke(view);
                    return CurrentHierarchyView;
                default:
                    return NoneView;
            }
        }

        public void RegisterOnViewChanged(Action<EditorViews.Views> onChange)
        {
            if (onViewChanged == null)
            {
                onViewChanged = onChange;
            }
            else
            {
                onViewChanged += onChange;
            }
        }
    }
}
