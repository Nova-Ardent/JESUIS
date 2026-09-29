using JESUIS.Shared.ScreenData.DataBindings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JESUIS.Runtime.Screen.Data
{
    public class Model
    {
        int lastUpdate = -1;
        Dictionary<System.Guid, ModelDataWrapper> dataContainers = new Dictionary<System.Guid, ModelDataWrapper>(20);
        public Model[] Children { get; private set; } = new Model[0];

#if UNITY_EDITOR
        public string Name { get; private set; }
#endif

        public Model(params Model[] children)
        {
            SetChildren(children);
        }

#if UNITY_EDITOR
        public Model(string name, params Model[] children)
            : this(children)
        {
            this.Name = name;
        }
#endif

        public void SetChildren(params Model[] children)
        {
            SetChildren(children as IEnumerable<Model>);
        }

        public void SetChildren(IEnumerable<Model> children)
        {
            this.Children = children.ToArray();
        }

        public Model TryGetChildOrSelf(int index)
        {
            if (Children.Length == 0 || index >= Children.Length)
            {
                return this;
            }
            else
            {
                return Children[index];
            }
        }

        public void SetData<T>(DataBinding<T> dataBinding, T value)
        {
            lastUpdate = Time.frameCount;
            GetDataWrapper<T>(dataBinding).CurrentValue = value;
        }

        public T GetData<T>(DataBinding<T> dataBinding)
        {
            return GetDataWrapper<T>(dataBinding).CurrentValue;
        }

        public ModelDataWrapper<T> GetDataWrapper<T>(DataBinding<T> dataBinding)
        {
            return GetDataWrapper<T>(dataBinding.UID);
        }

        public ModelDataWrapper<T> GetDataWrapper<T>(System.Guid uid)
        {
            if (dataContainers.TryGetValue(uid, out ModelDataWrapper value))
            {
                return (ModelDataWrapper<T>)value;
            }

            ModelDataWrapper<T> modelDataWrapper = new ModelDataWrapper<T>();
            dataContainers[uid] = modelDataWrapper;
            return modelDataWrapper;
        }

        public int LastUpdate()
        {
            return lastUpdate;
        }
    }
}
