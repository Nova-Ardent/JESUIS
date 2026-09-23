using static JESUIS.Editor.Elements.Input.LabelledFieldElement;
using UnityEngine;

namespace JESUIS.Editor.Elements.Input.Bindable
{
    public class BindableVector2fFieldElement : BindableInputFieldElement<Vector2, float, float>
    {
        public BindableVector2fFieldElement(string label, string XLabel = "X", string YLabel = "Y")
            : base(new Vector2fFieldElement(label, XLabel, YLabel, elementType: LabelType.MainUnpadded))
        {
        }

        public override float GetSubType1(Vector2 bindingValue)
        {
            return bindingValue.x;
        }

        public override float GetSubType2(Vector2 bindingValue)
        {
            return bindingValue.y;
        }

        public override Vector2 BuildBindingType(float s1, float s2)
        {
            return new Vector2(s1, s2);
        }
    }

    public class BindableVector2iFieldElement : BindableInputFieldElement<Vector2Int, int, int>
    {
        public BindableVector2iFieldElement(string label)
            : base(new Vector2iFieldElement(label, LabelType.MainUnpadded))
        {
        }

        public override int GetSubType1(Vector2Int bindingValue)
        {
            return bindingValue.x;
        }

        public override int GetSubType2(Vector2Int bindingValue)
        {
            return bindingValue.y;
        }

        public override Vector2Int BuildBindingType(int s1, int s2)
        {
            return new Vector2Int(s1, s2);
        }
    }
}