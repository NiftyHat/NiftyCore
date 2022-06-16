using System;
using UnityEngine;

namespace NiftyFramework.Core.Data
{
    /*
     * Optional serialized prop. Based on examples from https://gist.github.com/aarthificial
     * requires unity 2020.1 or later.
     */
    [Serializable]
    public struct Optional<TValue> : IOptional
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private TValue _value;

        public bool Enabled => _enabled;
        public TValue Value => _value;

        public Optional(TValue value)
        {
            _value = value;
            _enabled = true;
        }

        public bool TryGet(out TValue output)
        {
            if (_enabled)
            {
                output = _value;
                return true;
            }
            output = default;
            return false;
        }
        
        public void TrySet(ref TValue output)
        {
            if (_enabled)
            {
                output = _value;
            }
        }
    }

    public interface IOptional
    {
        public bool Enabled { get; }
    }
}