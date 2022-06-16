using System;
using UnityEngine;

namespace NiftyFramework.Core.Comparison
{
    [Serializable]
    public class SelectableComparison<TValue> : ISerializationCallbackReceiver, IComparison<TValue> where TValue : IComparable<TValue>
    {
        [SerializeField] private string _selected;
        private IComparison<TValue> _method;
        public string Symbol => _method.Symbol;

        public bool Compare(TValue left, TValue right)
        {
            return _method.Compare(left, right);
        }
        
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _method = NiftyComparison<TValue>.StringToComparison(_selected);
        }
    }
}