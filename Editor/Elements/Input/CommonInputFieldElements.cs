namespace JESUIS.Editor.Elements.Input
{
    public class TextInputFieldElement : InputFieldElement<string>
    {
        public TextInputFieldElement(string labelText, string defaultValue, bool isSubField = false) : base(labelText, defaultValue, isSubField)
        {
        }

        protected override string Convert(string value)
        {
            return value;
        }
    }

    public class IntInputFieldElement : InputFieldElement<int>
    {
        public IntInputFieldElement(string labelText, int defaultValue, bool isSubField = false) : base(labelText, defaultValue, isSubField)
        {
        }

        protected override bool IsValueValid(string value)
        {
            return int.TryParse(value, out _);
        }

        protected override int Convert(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return 0;
        }
    }

    public class FloatInputFieldElement : InputFieldElement<float>
    {
        public FloatInputFieldElement(string labelText, float defaultValue, bool isSubField = false) : base(labelText, defaultValue, isSubField)
        {
        }

        protected override bool IsValueValid(string value)
        {
            return float.TryParse(value, out _);
        }
        protected override float Convert(string value)
        {
            if (float.TryParse(value, out float result))
            {
                return result;
            }
            return 0f;
        }
    }
}