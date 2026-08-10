using JESUIS.Editor.UIBuilder.Data;
using System;

namespace JESUIS.Editor.UIBuilder.Panels.Views
{
    public class ViewManager
    {
        EditorState editorState;

        Action<EditorViews.Views> onViewChanged;

        public EditorViews NoneView { get => new EditorViews(editorState); }
        public HierarchyView CurrentHierarchyView { get; private set; }
        public InspectorView CurrentInspectorView { get; private set; }
        public RendererView CurrentRendererView { get; private set; }
        public FileView CurrentFileView { get; private set; }

        public ViewManager(EditorState editorState)
        {
            this.editorState = editorState;

            CurrentHierarchyView = new HierarchyView(editorState);
            CurrentInspectorView = new InspectorView(editorState);
            CurrentRendererView = new RendererView(editorState);
            CurrentFileView = new FileView(editorState);
        }

        public EditorViews GetView(EditorViews.Views view, bool triggerOnChange = true)
        {
            switch (view)
            {
                case EditorViews.Views.File:
                    onViewChanged?.Invoke(view);
                    return CurrentFileView;

                case EditorViews.Views.Hierarchy:
                    onViewChanged?.Invoke(view);
                    return CurrentHierarchyView;
                
                case EditorViews.Views.Inspector:
                    onViewChanged?.Invoke(view);
                    return CurrentInspectorView;

                case EditorViews.Views.Renderer:
                    onViewChanged?.Invoke(view);
                    return CurrentRendererView;
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
