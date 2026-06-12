using JESUIS.Editor.Elements.Common.Interfaces;
using JESUIS.Editor.Elements.Common.Widgets;
using UnityEngine.UIElements;

namespace JESUIS.Editor.Elements.Common.Layout
{
    public class SplittablePanel : VisualElement
        , IResizable
    {
        public const int NESTED_SPLIT_PADDING_SIZE = 0;
        public const int MIN_MAX_DRAGBAR_PADDING = 15;

        float _width = 0;
        float _height = 0;

        public bool IsSplit { get; private set; } = false;
        bool isSplitVertically = false;

        VisualElement elementOne = null;
        VisualElement elementTwo = null;

        DragBar panelBar = null;

        public SplittablePanel()
        {
        }

        public void SetToInitialState<T>(T element) where T : VisualElement, IResizable
        {
            if (elementOne != null)
            {
                Remove(elementOne);
            }
            elementOne = element;
            Add(elementOne);

            if (elementTwo != null)
            {
                Remove(elementTwo);
                elementTwo = null;
            }

            if (panelBar != null)
            {
                Remove(panelBar);
                panelBar = null;
            }

            IResizable resizable = elementOne as IResizable;
            resizable.Resize(_width, _height);
        }

        public void SplitVertically<T>(T newElement) where T : VisualElement, IResizable
        {
            elementTwo = newElement;
            Add(elementTwo);

            panelBar = new DragBar(true);
            panelBar.Resize(_width, _height);
            Add(panelBar);
            panelBar.RegisterCallbacks(SetStyleForVerticalSplit);

            IsSplit = true;
            isSplitVertically = true;
            SetStyleForVerticalSplit();
        }

        public void SplitHorizontally<T>(T newElement) where T : VisualElement, IResizable
        {
            elementTwo = newElement;
            Add(elementTwo);

            panelBar = new DragBar(false);
            panelBar.Resize(_width, _height);
            Add(panelBar);
            panelBar.RegisterCallbacks(SetStyleForHorizontalSplit);

            IsSplit = true;
            isSplitVertically = false;
            SetStyleForHorizontalSplit();
        }

        public void Resize(float width, float height)
        {
            _width = width;
            _height = height;

            style.width = width;
            style.height = height;

            if (!IsSplit)
            {
                if (elementOne is IResizable resizableOne)
                {
                    resizableOne.Resize(width, height);
                }

                if (elementTwo is IResizable resizableTwo)
                {
                    resizableTwo.Resize(width, height);
                }
            }
            else
            {
                panelBar.Resize(_width, _height);

                if (isSplitVertically)
                {
                    panelBar.SetBounds(-_width / 2 + MIN_MAX_DRAGBAR_PADDING, _width / 2 - MIN_MAX_DRAGBAR_PADDING);
                    SetStyleForVerticalSplit();
                }
                else
                {
                    panelBar.SetBounds(-_height / 2 + MIN_MAX_DRAGBAR_PADDING, _height / 2 - MIN_MAX_DRAGBAR_PADDING);
                    SetStyleForHorizontalSplit();
                }
            }
        }

        void SetStyleForVerticalSplit()
        {
            int padding = 0;
            if (parent is SplittablePanel panel)
            {
                padding = NESTED_SPLIT_PADDING_SIZE;
            }

            elementOne.style.position = Position.Absolute;
            if (elementOne is IResizable resizeableOne)
            {
                resizeableOne.Resize
                    ( _width / 2 - padding - DragBar.DRAG_BAR_TOTAL_SIZE / 2 + panelBar.GetDragPosition()
                    , _height - padding * 2
                    );
            }
                
            elementOne.style.left = padding;
            elementOne.style.top = padding;


            panelBar.style.position = Position.Absolute;
            panelBar.style.left = _width / 2 - DragBar.DRAG_BAR_TOTAL_SIZE / 2 + DragBar.DRAG_BAR_EDGE_PADDING + panelBar.GetDragPosition();
            panelBar.style.top = DragBar.DRAG_BAR_END_PADDING;


            elementTwo.style.position = Position.Absolute;
            if (elementTwo is IResizable resizeableTwo)
            {
                resizeableTwo.Resize
                    ( _width / 2 - padding - DragBar.DRAG_BAR_TOTAL_SIZE / 2 - panelBar.GetDragPosition()
                    , _height - padding * 2
                    );
            }

            elementTwo.style.left = _width / 2 + DragBar.DRAG_BAR_TOTAL_SIZE / 2 + panelBar.GetDragPosition();
            elementTwo.style.top = padding;
        }

        void SetStyleForHorizontalSplit()
        {
            int padding = 0;
            if (parent is SplittablePanel panel)
            {
                padding = NESTED_SPLIT_PADDING_SIZE;
            }

            elementOne.style.position = Position.Absolute;
            if (elementOne is IResizable resizeableOne)
            {
                resizeableOne.Resize
                    ( _width - padding * 2
                    , _height / 2 - padding - DragBar.DRAG_BAR_TOTAL_SIZE / 2 + panelBar.GetDragPosition()
                    );
            }

            elementOne.style.left = padding;
            elementOne.style.top = padding;


            panelBar.style.position = Position.Absolute;
            panelBar.style.left = DragBar.DRAG_BAR_END_PADDING;
            panelBar.style.top = _height / 2 - DragBar.DRAG_BAR_TOTAL_SIZE / 2 + DragBar.DRAG_BAR_EDGE_PADDING + panelBar.GetDragPosition();


            elementTwo.style.position = Position.Absolute;
            if (elementTwo is IResizable resizeableTwo)
            {
                resizeableTwo.Resize
                    ( _width - padding * 2
                    , _height / 2 - padding - DragBar.DRAG_BAR_TOTAL_SIZE / 2 - panelBar.GetDragPosition()
                    );
            }

            elementTwo.style.left = padding;
            elementTwo.style.top = _height / 2 + DragBar.DRAG_BAR_TOTAL_SIZE / 2 + panelBar.GetDragPosition();
        }
    }
}
