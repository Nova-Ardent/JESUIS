using JESUIS.Shared.ScreenData.Data;

namespace JESUIS.Editor.UIBuilder.Data.StateChanges
{
    public abstract class ElementChanges
    {
        public enum ElementChangeType
        {
            ValueUpdated,
            ChildAdded,
        }

        public abstract ElementChangeType ChangeType { get; }

        public BaseElement TargetElement { get; private set; }

        public ElementChanges(BaseElement targetElement)
        {
            TargetElement = targetElement;
        }
    }

    public abstract class ElementChanges<T> : ElementChanges
    {
        public T Data { get; private set; }

        public ElementChanges(BaseElement targetElement, T data) : base(targetElement)
        {
            Data = data;
        }
    }

    public class ValuesUpdated : ElementChanges
    {
        public override ElementChangeType ChangeType => ElementChangeType.ValueUpdated;

        public ValuesUpdated(BaseElement target) : base(target)
        {
        }
    }

    public class ChildAdded : ElementChanges<BaseElement>
    {
        public override ElementChangeType ChangeType => ElementChangeType.ChildAdded;

        public ChildAdded(BaseElement target, BaseElement childElement) : base(target, childElement)
        {
        }
    }
}
