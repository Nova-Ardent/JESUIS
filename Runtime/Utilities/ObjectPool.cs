using System.Collections.Generic;
using UnityEngine;

namespace JESUIS.Runtime.Utilities
{
    public class ObjectPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        private T prefab;
        private int maxcapacity;
        private GameObject poolContainer;

        public Stack<T> pool = new Stack<T>();
        public List<T> active = new List<T>();

        public ObjectPool(T prefab, int maxCount = 0, GameObject poolContainer = null)
        {
            this.prefab = prefab;
            this.maxcapacity = maxCount;
            this.poolContainer = poolContainer;
        }

        public T Instantiate()
        {
            if (pool.Count > 0)
            {
                T obj = pool.Pop();
                obj.gameObject.SetActive(true);
                return obj;
            }

            T ret = GameObject.Instantiate(prefab.gameObject).GetComponent<T>();
            ret.SetPool(this);
            active.Add(ret);
            return ret;
        }

        public void Release(T obj)
        {
            active.Remove(obj);
            ReleaseInternal(obj);
        }

        void ReleaseInternal(T obj)
        {
            if (maxcapacity != 0 && pool.Count >= maxcapacity)
            {
                GameObject.Destroy(obj.gameObject);
                return;
            }

            obj.gameObject.SetActive(false);
            pool.Push(obj);

            if (poolContainer != null)
            {
                obj.transform.SetParent(poolContainer.transform);
            }
        }
    }
}
