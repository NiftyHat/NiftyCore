namespace NiftyFramework.Core.Condition
{
    public abstract class StatefulCondition
    {
        public delegate void DelegateChanged(bool isConditionMet, StatefulCondition condition);

        public delegate void DelegateMet(StatefulCondition condition);

        public event DelegateChanged OnStateChanged;
        public event DelegateMet OnConditionMet;
        protected bool _isMet;

        public bool isMet()
        {
            return _isMet;
        }

        protected bool StateChange(bool newState)
        {
            if (_isMet != newState)
            {
                _isMet = newState;
                OnStateChanged?.Invoke(_isMet, this);
                if (_isMet)
                {
                    OnConditionMet?.Invoke(this);
                }
            }
            return _isMet;
        }

        public void AddListenerOnce(DelegateMet callback)
        {
            if (_isMet)
            {
                callback.Invoke(this);
            }
            else
            {
                OnConditionMet += (item) =>
                {
                    callback.Invoke(this);
                    OnConditionMet -= callback;
                };
            }
        }
    }
}