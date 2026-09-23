using UnityEngine;

namespace JESUIS.Shared.ScreenData.DataBindings
{
    public interface IBindable
    {
        public ulong UIDHigh { get; set; }
        public ulong UIDLow { get; set; }

        public void SetBindingValue(object value);
        public object GetBindingValue();
    }

    [System.Serializable]
    public struct Bindable<T> : IBindable
    {
        [SerializeField] public T Value;
        
        [SerializeField] ulong uIDHigh;
        public ulong UIDHigh 
        {
            get => uIDHigh;
            set 
            {
                uIDHigh = value; 
            } 
        }

        [SerializeField] ulong uIDLow;
        public ulong UIDLow
        {
            get => uIDLow;
            set
            {
                uIDLow = value;
            }
        }

        public void SetBindingValue(object value)
        {
            if (typeof(T).IsEnum)
            {
                Value = (T)System.Enum.ToObject(typeof(T), value);
            }
            else
            {
                Value = (T)value;
            }
        }

        public object GetBindingValue()
        {
            return Value;
        }
    }
}
