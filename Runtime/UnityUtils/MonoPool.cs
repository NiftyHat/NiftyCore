using System;
using System.Collections.Generic;
using NiftyFramework.UI;
using UnityEngine;

namespace UnityUtils
{
    [Serializable]
    public class MonoPool<TMonoBehavior> where TMonoBehavior : MonoBehaviour
    {
        public struct PoolItem
        {
            public GameObject Object;
            public TMonoBehavior MonoBehavior;
            public IView View;
        }

        public delegate void DelegateChange (TMonoBehavior monoBehavior);
        public delegate TMonoBehavior DelegateFactory();

        private List<PoolItem> _pooledItems = new List<PoolItem>();
        private DelegateFactory _factory;
        private int _maxSize;

        /// <summary>
        /// Creates a pool using the initial item. 
        /// </summary>
        /// <param name="initialItem">Prototype item. Used as a factory for other items</param>
        /// <param name="maxSize">Max size for the pool</param>
        public MonoPool(TMonoBehavior initialItem, int maxSize = -1)
        {
            if (initialItem != null && initialItem.gameObject != null)
            {
                var parent = initialItem.gameObject.transform.parent;
                initialItem.gameObject.SetActive(false);
                _factory = () => UnityEngine.Object.Instantiate(initialItem.gameObject, parent).GetComponent<TMonoBehavior>();
            }

            if (maxSize > _maxSize)
            {
                _maxSize = maxSize;
            }
        }
        
        public MonoPool(DelegateFactory factory, int maxSize = -1)
        {
            _pooledItems = new List<PoolItem>();
            _factory = factory;
            if (maxSize > _maxSize)
            {
                _maxSize = maxSize;
            }
        }

        public bool TryGet(out TMonoBehavior instance)
        {
            if (_pooledItems.Count <= 0)
            {
                instance = _factory();
                if (instance.gameObject == null)
                {
                    throw new NullReferenceException(
                        $"{nameof(MonoPool<TMonoBehavior>)} {nameof(TryGet)}() factory method {_factory.Method.Name} return null GameObject ref");
                }
                return instance;
            }
            int lastIndex = _pooledItems.Count - 1;
            PoolItem item =  _pooledItems[lastIndex];
            _pooledItems.RemoveAt(lastIndex);
            instance = item.MonoBehavior;
            item.Object.SetActive(true);
            return item.Object != null && item.MonoBehavior != null;
        }

        public bool TryReturn(TMonoBehavior instance)
        {
            if (_pooledItems.Count < _maxSize)
            {
                PoolItem poolItem = new PoolItem
                {
                    Object = instance.gameObject,
                    MonoBehavior = instance,
                    View = instance as IView
                };
                poolItem.View?.Clear();
                poolItem.Object.SetActive(false);
                _pooledItems.Add(poolItem);
                return true;
            }
            UnityEngine.Object.Destroy(instance.gameObject);
            return false;
        }

        [Obsolete("Don't use this until a proper Clear and Release function exists")]
        public void Clear()
        {
            foreach (var item in _pooledItems)
            {
                item.Object.SetActive(false);
                item.View?.Clear();
            }
            _pooledItems.Clear();
        }
    }
}