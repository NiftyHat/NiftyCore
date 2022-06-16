using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiftyFramework.SerializedDictionary
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct SerializedEntry
        {
            internal TKey Key;
            internal TValue Value;

            public SerializedEntry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public SerializedEntry(KeyValuePair<TKey, TValue> kvp)
            {
                Key = kvp.Key;
                Value = kvp.Value;
            }
        }

        [SerializeField] private List<SerializedEntry> _entries;

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            if (_entries != null)
            {
                _entries.Clear();
            }
            foreach(KeyValuePair<TKey, TValue> pair in this)
            {
                _entries.Add(new SerializedEntry(pair));
            }
        }
     
        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < _entries.Count; i++)
            {
                SerializedEntry serializedEntry = _entries[i];
                Add(serializedEntry.Key, serializedEntry.Value);
            }
        }
    }
}