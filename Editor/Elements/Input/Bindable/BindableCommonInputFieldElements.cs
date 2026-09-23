using JESUIS.Shared.ScreenData.DataBindings;
using static JESUIS.Editor.Elements.Input.LabelledFieldElement;

namespace JESUIS.Editor.Elements.Input.Bindable
{
    public class BindableTextInputFieldElement : BindableInputFieldElement<string>
    {
        public BindableTextInputFieldElement(string labelText, string defaultValue, bool isReadonly = false) 
            : base(new TextInputFieldElement(labelText, defaultValue, LabelType.MainUnpadded, isReadonly))
        {
        }

        public void SetTextBinding(Bindable<string> bindable)
        {
            bindable.Value = bindable.Value ?? "";
            Set(bindable);
        }

        public void SetTextBindingWithoutNotify(Bindable<string> bindable)
        {
            bindable.Value = bindable.Value ?? "";
            SetWithoutNotify(bindable);
        }
    }

    public class BindableIntInputFieldElement : BindableInputFieldElement<int>
    {
        public BindableIntInputFieldElement(string labelText, int defaultValue)
            : base(new IntInputFieldElement(labelText, defaultValue, LabelType.MainUnpadded))
        { 
        }
    }

    public class BindableFloatInputFieldElement : BindableInputFieldElement<float>
    {
        public BindableFloatInputFieldElement(string labelText, float defaultValue)
            : base(new FloatInputFieldElement(labelText, defaultValue, LabelType.MainUnpadded))
        {
        }
    }
}