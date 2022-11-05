using System;
using System.Collections.Generic;
using NiftyFramework.UI;
using UnityEngine;

namespace UnityUtils
{
    public class MonoPool<TMonoBehavior> where TMonoBehavior : MonoBehaviour
    {
        public delegate TMonoBehavior MonoBehaviorInstanced();

        private readonly HashSet<TMonoBehavior> _pooledItems = new HashSet<TMonoBehavior>();
        private readonly MonoBehaviorInstanced _instanceFunction;
        private readonly int _maxSize = int.MaxValue;
        private TMonoBehavior _first;
        public int Count => _pooledItems.Count;
        
        private MonoPool(int maxSize = 0)
        {
            if (maxSize >= 1)
            {
                _maxSize = maxSize;
            }
        }
        
        private MonoPool(int maxSize = -1, int initialSize = -1) : this(maxSize)
        {
            if (initialSize > 0)
            {
                if (initialSize > maxSize)
                {
                    Debug.LogWarning($"{nameof(initialSize)} {initialSize} has clamped to {nameof(maxSize)} of {maxSize}");
                }
            }
        }

        /// <summary>
        /// Creates a pool using the initial item. 
        /// </summary>
        /// <param name="initialItem">Prototype item. Used as a factory for other items</param>
        /// <param name="maxSize">Max size for the pool</param>
        /// <param name="initialSize">Initial size for the pool. Allocates items using Prewarm</param>
        public MonoPool(TMonoBehavior initialItem, int maxSize = -1, int initialSize = -1) : this(maxSize, initialSize)
        {
            if (initialItem != null && initialItem.gameObject != null)
            {
                var parent = initialItem.gameObject.transform.parent;
                initialItem.gameObject.SetActive(false);
                _instanceFunction = () => UnityEngine.Object.Instantiate(initialItem.gameObject, parent).GetComponent<TMonoBehavior>();
            }
            Prewarm(initialSize);
        }
        
        /// <summary>
        /// Creates a pool using a factory method.
        /// </summary>
        /// <param name="instanceFunction">Factory the creates new pool items</param>
        /// <param name="maxSize">Maximum pool size. If Count is greater than Size objects cannot be sent to the pool</param>
        /// <param name="initialSize">Initial size of the pool. Invokes Prewarm that will create the objects</param>
        public MonoPool(MonoBehaviorInstanced instanceFunction, int maxSize = -1, int initialSize = -1) : this(maxSize, initialSize)
        {
            _pooledItems = new HashSet<TMonoBehavior>();
            _instanceFunction = instanceFunction;
            if (maxSize > _maxSize)
            {
                _maxSize = maxSize;
            }
            Prewarm(initialSize);
        }

        private void Prewarm(int itemCount)
        {
            for (int i = 0; i < itemCount && i < _maxSize; i++)
            {
                var instance = _instanceFunction();
                if (instance is IView view)
                {
                    view.Clear();
                }
                if (instance.gameObject != null)
                {
                    instance.gameObject.SetActive(false);
                }
                _pooledItems.Add(instance);
            }
            _first = GetHead();
        }

        private TMonoBehavior GetHead()
        {
            if (_pooledItems.Count == 0)
            {
                return null;
            }
            foreach (var pooled in _pooledItems)
            {
                if (pooled != null)
                {
                    return pooled;
                }
            }
            return null;
        }
        
        public bool TryGet(out TMonoBehavior instance)
        {
            if (_first == null)
            {
                instance = _instanceFunction();
                if (instance.gameObject == null)
                {
                    throw new NullReferenceException(
                        $"{nameof(MonoPool<TMonoBehavior>)} {nameof(TryGet)}() factory method {_instanceFunction.Method.Name} return null GameObject ref");
                }
                return instance.TrySetActive(true);
            }
            _pooledItems.Remove(_first);
            instance = _first;
            _first = GetHead();
            return instance.TrySetActive(true);
        }
        
        public bool TryGet(out HashSet<TMonoBehavior> instanceList, int count = 1)
        {
            if (count <= 0)
            {
                instanceList = new HashSet<TMonoBehavior>();
                return true;
            }
            if (count == 1)
            {
                if (TryGet(out var instance))
                {
                    instanceList = new HashSet<TMonoBehavior>() {instance};
                    return true;
                }
                instanceList = null;
            }
            instanceList = new HashSet<TMonoBehavior>();
            if (_pooledItems.Count > 0)
            {
                var enumerator = _pooledItems.GetEnumerator();
                while (enumerator.MoveNext() && count > 0)
                {
                    var instance = enumerator.Current;
                    instanceList.Add(instance);
                    if (_first == instance)
                    {
                        _first = null;
                    }
                    count--;
                }
                _pooledItems.ExceptWith(instanceList);
                enumerator.Dispose();
            }
            while (instanceList.Count < count)
            {
                if (TryGet(out var instance))
                {
                    instanceList.Add(instance);
                }
            }
            return instanceList.Count == count;
        }

        public bool TryReturn(TMonoBehavior instance)
        {
            if (_pooledItems.Contains(instance))
            {
                return false;
            }
            if (_pooledItems.Count < _maxSize)
            {
                if (instance is IView view)
                {
                    view.Clear();
                }
                instance.TrySetActive(false);
                _pooledItems.Add(instance);
                if (_first == null)
                {
                    _first = instance;
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Trys to return everything in the given list into the pool. If an item is already in the pool or the pool is
        /// at capacity it will leave that item in the passed in HashSet.
        /// </summary>
        /// <param name="instanceList">Items to return. If there are more items that the pool has capacity for they will stay in the list</param>
        /// <returns>true if all items where returned false if there are items left</returns>
        public bool TryReturn(HashSet<TMonoBehavior> instanceList)
        {
            HashSet<TMonoBehavior> removedItems = new HashSet<TMonoBehavior>();
            foreach (var instance in instanceList)
            {
                if (TryReturn(instance))
                {
                    removedItems.Add(instance);
                }
            }
            instanceList.ExceptWith(removedItems);
            return instanceList.Count == 0;
        }
        
        /// <summary>
        /// Dispose that allows you to do custom cleanup on your object. You should still invoke Destroy/DestroyImmediate inside your callback
        /// </summary>
        /// <param name="onDispose">Callback on the item being disposed. After this is called MonoPool will destroy the internal ref making it orphaned</param>
        /// <exception cref="ArgumentException">Throws an except if you don't assign a dispose function, because that will create orphaned references</exception>
        public void Dispose(Action<TMonoBehavior> onDispose)
        {
            if (onDispose == null)
            {
                throw new ArgumentException(
                    $"Calling {nameof(Dispose)}() without a handler will clear references without removing Unity object.");
            }
            _first = null;
            foreach (var item in _pooledItems)
            {
                onDispose(item);
            }
            _pooledItems.Clear();
        }

        public void Dispose()
        {
            _first = null;
            foreach (var item in _pooledItems)
            {
                if (item != null && item.gameObject != null)
                {
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }
            _pooledItems.Clear();
        }
    }
}