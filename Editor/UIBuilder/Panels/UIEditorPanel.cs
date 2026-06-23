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
        ViewManager viewManager;

        TabBar tabBar;
        VisualElement content;
        EditorViews currentView;
        
        public UIEditorPanel(ViewManager viewManager) : this(viewManager, 0, 0)
        {
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
                    }
                    , true);
                return action;
            }).ToList();

            DropDown dropDown = new DropDown(150, namedActions);
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

                bottomSplit.SetToInitialState(new UIEditorPanel(viewManager));
                topSplit.SetToInitialState(this);
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

                leftSplit.SetToInitialState(new UIEditorPanel(viewManager));
                rightSplit.SetToInitialState(this);
            }
        }

        protected override IEnumerable<NamedAction> GetContextMenuOptions()
        {
            if (this.parent is SplittablePanel panel)
            {
                yield return new NamedAction("Split Vertically", SplitVertically, true);
                yield return new NamedAction("Split Horizontally", SplitHorizontally, true);
            }
        }
    }
}
