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

        /// <summary>
        /// Creates a pool using the initial item. 
        /// </summary>
        /// <param name="initialItem"></param>
        public MonoPool(TMonoBehavior initialItem)
        {
            if (initialItem != null && initialItem.gameObject != null)
            {
                var parent = initialItem.gameObject.transform.parent;
                initialItem.gameObject.SetActive(false);
                _factory = () => GameObject.Instantiate(initialItem.gameObject, parent).GetComponent<TMonoBehavior>();
            }
        }
        
        public MonoPool(DelegateFactory factory)
        {
            _pooledItems = new List<PoolItem>();
            _factory = factory;
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
                _pooledItems.Add(new PoolItem
                {
                    Object = instance.gameObject,
                    MonoBehavior = instance
                });
            }
            int lastIndex = _pooledItems.Count - 1;
            PoolItem item =  _pooledItems[lastIndex];
            instance = item.MonoBehavior;
            item.Object.SetActive(true);
            _pooledItems.RemoveAt(lastIndex);
            return item.Object != null && item.MonoBehavior != null;
        }

        public void TryReturn(TMonoBehavior instance)
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
        }

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