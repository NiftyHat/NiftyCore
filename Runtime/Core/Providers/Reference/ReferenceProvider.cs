namespace NiftyFramework.Core.Providers.Reference
{
    public class ReferenceProvider<TReference>
    {
        public delegate void Changed(TReference oldValue, TReference newValue);
        public delegate void Updated(bool isAssigned);

        public event Changed OnChanged;
        public event Updated OnUpdated;
        
        protected bool _isSet = false;
        
        protected TReference _reference;
        public TReference Reference
        {
            get => _reference;
            set => Set(value);
        }

        public bool IsSet => _isSet;

        public TReference Get(Changed onChanged)
        {
            OnChanged += onChanged;
            return _reference;
        }

        protected virtual TReference Set(TReference newValue)
        {
            if (OnChanged == null && OnUpdated == null)
            {
                //TODO dsaunders - log this as bad behavior? It's wasteful using this wrapper without listeners.
                _reference = newValue;
                return _reference;
            }

            if (OnChanged != null)
            {
                TReference lastValue = _reference;
                OnChanged?.Invoke(newValue, lastValue);
                _reference = newValue;
            }

            if (OnUpdated != null)
            {
                _isSet = _reference != null;
                OnUpdated(_isSet);
            }
            return _reference;
        }

        public ReferenceProvider ()
        {
            _isSet = false;
        }
        
        public ReferenceProvider (TReference initialReference)
        {
            Reference = initialReference;
            _isSet = initialReference != null;
        }
    }
}