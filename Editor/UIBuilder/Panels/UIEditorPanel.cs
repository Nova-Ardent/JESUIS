using JESUIS.Editor.Elements.Common.Layout;
using JESUIS.Editor.Elements.Common.Panel;
using JESUIS.Editor.Helpers;
using System.Collections.Generic;

namespace JESUIS.Editor.UIBuilder.Panels
{
    public class UIEditorPanel : BasePanel
    {
        public UIEditorPanel() : this(0, 0)
        { 
        }

        public UIEditorPanel(float width, float height) : base(width, height)
        {
            style.backgroundColor = Settings.Colors.PANEL_COLOR;

            TabBar tabBar = new TabBar();
            Add(tabBar);
        }

        private void SplitVertically()
        {
            if (this.parent is SplittablePanel parentPanel)
            {
                SplittablePanel topSplit = new SplittablePanel();
                SplittablePanel bottomSplit = new SplittablePanel();
                parentPanel.SetToInitialState(topSplit);
                parentPanel.SplitVertically(bottomSplit);

                bottomSplit.SetToInitialState(new UIEditorPanel());
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

                leftSplit.SetToInitialState(new UIEditorPanel());
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
