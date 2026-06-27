using JESUIS.Editor.Elements.Common.Layout;
using JESUIS.Editor.Elements.Common.Layout.TabBarWidgets;
using JESUIS.Editor.Elements.Common.Panel;
using JESUIS.Editor.Helpers;
using JESUIS.Editor.UIBuilder.Panels.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using static JESUIS.Editor.Utilities.Utilities;

namespace JESUIS.Editor.UIBuilder.Panels
{
    public class UIEditorPanel : BasePanel
    {
        UIEditorLayoutManager layoutManager;
        ViewManager viewManager;

        TabBar tabBar;
        DropDown dropDown;

        VisualElement content;
        EditorViews currentView;
        
        public UIEditorPanel(ViewManager viewManager, UIEditorLayoutManager layoutManager) : this(viewManager, 0, 0)
        {
            this.layoutManager = layoutManager;
        }

        public UIEditorPanel(ViewManager viewManager, float width, float height) : base(width, height)
        {
            this.viewManager = viewManager;

            style.backgroundColor = Settings.Colors.PANEL_COLOR;

            tabBar = new TabBar();
            tabBar.style.position = Position.Absolute;
            Add(tabBar);

            content = new VisualElement();
            content.style.position = Position.Absolute;
            content.style.left = 0;
            content.style.top = TabBar.COMMON_TAB_BAR_HEIGHT;
            Add(content);

            SetViewOptions();
        }

        public override void Resize(float width, float height)
        {
            base.Resize(width, height);
            content.style.width = width;
            content.style.height = height - TabBar.COMMON_TAB_BAR_HEIGHT;

            layoutManager.QueueEditorPreferenceUpdate();
        }

        public EditorViews.Views GetView()
        {
            return currentView.Type;
        }

        public void SetView(EditorViews.Views view)
        {
            currentView = viewManager.GetView(view);
            content.Clear();

            if (currentView.Type != EditorViews.Views.None)
            {
                content.Add(currentView);
            }

            dropDown.SetOption((int)view);
        }

        void SetViewOptions()
        {
            currentView = viewManager.NoneView;

            List<NamedAction> namedActions = GetEnums<EditorViews.Views>().Select(view =>
            {
                NamedAction action = new NamedAction
                    ( view.ToString()
                    , () =>
                    {
                        currentView = viewManager.GetView(view);
                        content.Clear();

                        if (currentView.Type != EditorViews.Views.None)
                        {
                            content.Add(currentView);
                        }

                        layoutManager.QueueEditorPreferenceUpdate();
                    }
                    , true);
                return action;
            }).ToList();

            dropDown = new DropDown(150, namedActions);
            tabBar.Add(dropDown);

            viewManager.RegisterOnViewChanged(view =>
            {
                if (view == currentView.Type)
                {
                    dropDown.SetOption(0);
                }
            });
        }

        private void SplitVertically()
        {
            if (this.parent is SplittablePanel parentPanel)
            {
                SplittablePanel topSplit = new SplittablePanel();
                SplittablePanel bottomSplit = new SplittablePanel();
                parentPanel.SetToInitialState(topSplit);
                parentPanel.SplitVertically(bottomSplit);

                bottomSplit.SetToInitialState(new UIEditorPanel(viewManager, layoutManager));
                topSplit.SetToInitialState(this);

                layoutManager.QueueEditorPreferenceUpdate();
            }
        }

        private void SplitHorizontally()
        {
            if (this.parent is SplittablePanel parentPanel)
            {
                SplittablePanel leftSplit = new SplittablePanel();
                SplittablePanel rightSplit = new SplittablePanel();
                parentPanel.SetToInitialState(leftSplit);
                parentPanel.SplitHorizontally(rightSplit);

                leftSplit.SetToInitialState(new UIEditorPanel(viewManager, layoutManager));
                rightSplit.SetToInitialState(this);

                layoutManager.QueueEditorPreferenceUpdate();
            }
        }

        private void CollapseWindow()
        {
            var splitPanel = this?.parent?.parent;
            if (splitPanel is SplittablePanel splittablePanel && splittablePanel != null && splittablePanel.IsSplit)
            {
                if (this.parent is SplittablePanel parentPanel)
                {
                    splittablePanel.Collapse(parentPanel, true);

                    layoutManager.QueueEditorPreferenceUpdate();
                }
            }
        }

        protected override IEnumerable<NamedAction> GetContextMenuOptions()
        {
            if (this.parent is SplittablePanel panel)
            {
                yield return new NamedAction("Split Vertically", SplitVertically, true);
                yield return new NamedAction("Split Horizontally", SplitHorizontally, true);

                var splitPanel = this?.parent?.parent;
                if (splitPanel is SplittablePanel splittablePanel && splittablePanel != null && splittablePanel.IsSplit)
                {
                    yield return new NamedAction("Collapse", CollapseWindow, true);
                }
            }
        }
    }
}
