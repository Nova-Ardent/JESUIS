using System;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.DataBindings
{
    public interface IBindable
    {
        public ulong UIDHigh { get; set; }
        public ulong UIDLow { get; set; }
        public bool HasUID { get; }
        public System.Guid Guid { get; }

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
                guid = null;
                uIDHigh = value; 
            } 
        }

        [SerializeField] ulong uIDLow;
        public ulong UIDLow
        {
            get => uIDLow;
            set
            {
                guid = null;
                uIDLow = value;
            }
        }

        public bool HasUID => UIDHigh != 0 || UIDLow != 0;

        [NonSerialized]
        System.Guid? guid;
        public System.Guid Guid
        {
            get
            {
                if (this.guid == null)
                {
                    Span<byte> bytes = stackalloc byte[16];
                    BitConverter.TryWriteBytes(bytes[..8], UIDHigh);
                    BitConverter.TryWriteBytes(bytes[8..], UIDLow);

                    guid = new System.Guid(bytes);
                }

                return guid.Value;
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
