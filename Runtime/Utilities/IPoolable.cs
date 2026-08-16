using UnityEngine;

namespace JESUIS.Runtime.Utilities
{
    public interface IPoolable<T> where T : MonoBehaviour, IPoolable<T>
    {
        public ObjectPool<T> owningPool { get; set; }

        public void SetPool(ObjectPool<T> objectPool)
        {
            owningPool = objectPool;
        }

        public void ReleaseToPool();
    }
}