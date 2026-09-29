using System;
using System.Collections.Generic;
using UnityEngine;

namespace JESUIS.Runtime.Screen.Data
{
    public abstract class ModelDataWrapper
    {
        public bool HasBeenSet { get; protected set; } = false;
        public object CurrentObjectValue;

        public abstract void RemoveListener(object key);
        public abstract int GetListenerCount();
    }

    public class ModelDataWrapper<T> : ModelDataWrapper
    {
        Dictionary<object, ModelDataWrapperListenerGroup<T>> listenerGroups = new Dictionary<object, ModelDataWrapperListenerGroup<T>>();

        public T CurrentValue 
        {
            get => (T)base.CurrentObjectValue;
            set
            {
                HasBeenSet = true;
                base.CurrentObjectValue = value;

                foreach (var listenerGroup in listenerGroups.Values)
                {
                    foreach (var listener in listenerGroup.listeners)
                    {
                        listener.Update(value);
                    }
                }
            }
        }

        public void AddListener(object key, ModelDataWrapperListener<T> listener)
        {
            if (!listenerGroups.TryGetValue(key, out var listenerGroup))
            {
                listenerGroup = new ModelDataWrapperListenerGroup<T>();
                listenerGroups[key] = listenerGroup;

                if (HasBeenSet)
                {
                    listener.Update(CurrentValue);
                }
            }

            listenerGroup.listeners.Add(listener);
        }

        public override void RemoveListener(object key)
        {
            listenerGroups.Remove(key);
        }

        public override int GetListenerCount()
        {
            return listenerGroups.Values.Count;
        }
    }

    public class ModelDataWrapperListenerGroup<T> 
    {
        public List<ModelDataWrapperListener<T>> listeners = new List<ModelDataWrapperListener<T>>();
    }

    public class ModelDataWrapperListener<T>
    {
        public T Value { get; private set; }

        public ModelDataWrapperListener(T baseValue)
        {
            Value = baseValue;
        }

        public void Update(T newValue)
        {
            Value = newValue;
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
