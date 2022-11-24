using System;
using NiftyFramework.Core.Condition;
using UnityEngine;

namespace NiftyFramework.Core
{
    public abstract class ValueProvider<TValue> : IValueProvider<TValue> where TValue : IComparable<TValue>
    {
        public delegate void Changed(TValue oldValue, TValue newValue);
        public delegate void Initialized(TValue initialValue);

        public event Changed OnChanged;
        public event Updated OnUpdated;
        public event Initialized OnInitialized;

        protected bool _isDefault = false;

        protected TValue _value;
        public TValue Value
        {
            get => _value;
            set => Set(value);
        }
        
        public virtual TValue Get(Initialized onInitialized = null, Changed onChanged = null)
        {
            onInitialized?.Invoke(_value);
            if (onChanged != null)
            {
                OnChanged += onChanged;
            }
            return _value;
        }
        
        public virtual TValue Get(Changed onChanged = null)
        {
            if (onChanged != null)
            {
                OnChanged += onChanged;
            }
            return _value;
        }

        protected virtual TValue Set(TValue newValue)
        {
            if (OnChanged == null && OnUpdated == null)
            {
                Debug.LogWarning($"{nameof(ValueProvider<TValue>)}.{nameof(Set)}() called but no watcher assigned. You could use an int instead");
            }

            if (newValue.Equals(_value))
            {
                if (_isDefault)
                {
                    _isDefault = false;
                    OnUpdated?.Invoke();
                }
                return _value;
            }
            
            OnUpdated?.Invoke();
            
            if (OnChanged != null)
            {
                TValue lastValue = _value;
                OnChanged?.Invoke(newValue, lastValue);
                _value = newValue;
            }
            return _value;
        }

        protected ValueProvider ()
        {
            _isDefault = true;
        }
        
        public ValueProvider (TValue initialValue)
        {
            _value = initialValue;
        }
        
        protected struct AmountCondition : ICondition
        {
            private TValue _target;
            private readonly IValueProvider<TValue> _valueProvider;
            private readonly IComparison<TValue> _comparison;
        
            public AmountCondition(IValueProvider<TValue> valueProvider, TValue target, IComparison<TValue> comparison)
            {
                _comparison = comparison;
                _valueProvider = valueProvider;
                _target = target;
            }

            public void SetTarget(TValue newTarget)
            {
                _target = newTarget;
            }

            public bool isMet()
            {
                return _comparison.Compare(_target, _valueProvider.Value);
            }
        }
    }
}