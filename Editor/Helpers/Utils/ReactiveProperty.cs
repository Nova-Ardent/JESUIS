using System;

namespace JESUIS.Editor.Helpers.Utils
{
    public class ReactiveProperty<T>
    {
        T _value;
        public T Value 
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    _onChange?.Invoke(_value);
                }
            }
        }

        Action<T> _onChange;

        public static implicit operator T(ReactiveProperty<T> reactiveProperty)
        {
            return reactiveProperty.Value;
        }

        public ReactiveProperty() : this(default(T))
        {
        }

        public ReactiveProperty(T value)
        {
            _value = value;
        }

        public void ListenTo(Action<T> onChange)
        {
            if (_onChange != null)
            {
                _onChange += onChange;
            }
            else
            {
                _onChange = onChange;
            }
        }
    }
}
