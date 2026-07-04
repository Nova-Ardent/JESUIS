using System;
using UnityEngine;

namespace JESUIS.Editor.Elements.Input
{
    public class Vector2fFieldElement : InputFieldElement<float, float>
    {
        public Vector2 CurrentValue { get; private set; }
        Action<Vector2> onValueChanged;

        public Vector2fFieldElement(string labelText) : base(labelText, "X", "Y")
        {
        }

        public void SetValueWithoutNotify(Vector2 newValue)
        {
            CurrentValue = newValue;
            SetValuesWithoutNotify(newValue.x, newValue.y);
        }

        protected override InputFieldElement<float> CreateInputOne(string subLabel, float defaultValue)
        {
            return new FloatInputFieldElement(subLabel, defaultValue, true);
        }

        protected override InputFieldElement<float> CreateInputTwo(string subLabel, float defaultValue)
        {
            return new FloatInputFieldElement(subLabel, defaultValue, true);
        }

        public void RegisterOnValueChanged(Action<Vector2> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
            }
            else
            {
                onValueChanged += onChange;
            }
        }

        protected override void OnChange(float i, float j)
        {
            CurrentValue = new Vector2(i, j);
            onValueChanged?.Invoke(CurrentValue);
        }
    }

    public class Vector2iFieldElement : InputFieldElement<int, int>
    {
        public Vector2Int CurrentValue { get; private set; }
        Action<Vector2Int> onValueChanged;

        public Vector2iFieldElement(string labelText) : base(labelText, "X", "Y")
        {
        }

        public void SetValueWithoutNotify(Vector2Int newValue)
        {
            CurrentValue = newValue;
            SetValuesWithoutNotify(newValue.x, newValue.y);
        }

        protected override InputFieldElement<int> CreateInputOne(string subLabel, int defaultValue)
        {
            return new IntInputFieldElement(subLabel, defaultValue, true);
        }

        protected override InputFieldElement<int> CreateInputTwo(string subLabel, int defaultValue)
        {
            return new IntInputFieldElement(subLabel, defaultValue, true);
        }

        public void RegisterOnValueChanged(Action<Vector2Int> onChange)
        {
            if (onValueChanged == null)
            {
                onValueChanged = onChange;
            }
            else
            {
                onValueChanged += onChange;
            }
        }

        protected override void OnChange(int i, int j)
        {
            CurrentValue = new Vector2Int(i, j);
            onValueChanged?.Invoke(CurrentValue);
        }
    }
}
