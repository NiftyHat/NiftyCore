using System;
using System.Collections.Generic;
using System.Linq;
using NiftyFramework.Tags;
using UnityEngine;

namespace NiftyFramework.Tags
{
    [Serializable]
    public class TagSet : ISerializationCallbackReceiver
    {
        [SerializeField] private string[] _serializedData;
        private HashSet<string> _items;
        public HashSet<string> Items => _items;
        
        public void Add(string item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                Array.Resize(ref _serializedData, _serializedData.Length + 1);
                _serializedData[_serializedData.Length - 1] = item;
            }
        }

        public void Add(IEnumerable<string> items)
        {
            _items.UnionWith(items);
            _serializedData = _items.ToArray();
        }

        public void Remove(IEnumerable<string> items)
        {
            _items.ExceptWith(items);
            _serializedData = _items.ToArray();
        }
        
        public void Remove(string item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
                _serializedData = _items.ToArray();
            }
        }

        public void OnBeforeSerialize()
        {
            if (_items != null)
            {
                _serializedData = _items.ToArray();
            }
        }

        public void OnAfterDeserialize()
        {
            if (_serializedData != null)
            {
                _items = new HashSet<string>(_serializedData);
            }
        }
    }
}