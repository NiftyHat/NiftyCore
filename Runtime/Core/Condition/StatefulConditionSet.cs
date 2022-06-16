namespace NiftyFramework.Core.Condition
{
    public class StatefulConditionSet : StatefulCondition
    {
        private StatefulCondition[] _conditions;
        private Mode _mode;

        public enum Mode
        {
            Any,
            All
        }
        
        public StatefulConditionSet(StatefulCondition[] conditions, Mode mode)
        {
            _conditions = conditions;
            foreach (var item in conditions)
            {
                item.OnStateChanged += HandleConditionUpdate;
            }
            _mode = mode;
        }
        
        public StatefulConditionSet(StatefulCondition condition, Mode mode)
        {
            _conditions = new[] { condition };
            foreach (var item in _conditions)
            {
                item.OnStateChanged += HandleConditionUpdate;
            }
            _mode = mode;
        }

        private void MatchAll(bool isMet, StatefulCondition conditionUpdated)
        {
            if (isMet == false)
            {
                StateChange(false);
            }
            foreach (var item in _conditions)
            {
                //don't check the condition that just updated;
                if (isMet && item == conditionUpdated)
                {
                    continue;
                }
                if (!item.isMet())
                {
                    StateChange(false);
                    return;
                }
            }
            StateChange(true);
        }
        
        private void MatchAny(bool isMet, StatefulCondition conditionUpdated)
        {
            if (isMet)
            {
                StateChange(true);
            }
            foreach (var item in _conditions)
            {
                //don't check the condition that just updated;
                if (isMet && item == conditionUpdated)
                {
                    continue;
                }
                if (item.isMet())
                {
                    StateChange(true);
                    return;
                }
            }
        }

        private void HandleConditionUpdate(bool isMet, StatefulCondition conditionUpdated)
        {
            if (_mode == Mode.All)
            {
                MatchAll(isMet, conditionUpdated);
            }
            if (_mode == Mode.Any)
            {
                MatchAny(isMet, conditionUpdated);
            }
        }
    }
}