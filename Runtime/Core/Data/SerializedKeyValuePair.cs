using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiftyFramework.Core.Data
{
    [Serializable]
    public class SerializedKeyValuePair<TKey,TValue>
    {
        [SerializeField] private TKey _key;
        [SerializeField] private TValue _value;

        public TKey Key => _key;
        public TValue Value => _value;

        public SerializedKeyValuePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        public SerializedKeyValuePair(KeyValuePair<TKey, TValue> kvp)
        {
            _key = kvp.Key;
            _value = kvp.Value;
        }
    }
}