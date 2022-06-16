using System.Collections.Generic;
using NiftyFramework.SerializedDictionary;
using UnityEngine;

namespace NiftyFramework.Tags
{
    public class TagDatabase : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<string, int> _tagCount = new SerializableDictionary<string, int>();

        public SerializableDictionary<string, int> GetTagCount()
        {
            return _tagCount;
        }
        
        public void Decrement(IEnumerable<string> tags)
        {
            foreach (string item in tags)
            {
                Decrement(item);
            }
        }

        public void Decrement(string tag)
        {
            if (_tagCount.TryGetValue(tag, out var value))
            {
                if (value == 1)
                {
                    _tagCount.Remove(tag);
                }
                _tagCount[tag] -= 1;
            }
        }

        public void Increment(string tag)
        {
            if (_tagCount.TryGetValue(tag, out var value))
            {
                _tagCount[tag] = value + 1;
            }
            else
            {
                _tagCount.Add(tag, 1);
            }
        }

        public void Increment(IEnumerable<string> tags)
        {
            foreach (string item in tags)
            {
                Increment(item);
            }
        }
    }
}