using NiftyFramework.Core.Condition;

namespace NiftyFramework.Milestones
{

    public abstract class Milestone
    {
        protected bool _isComplete;
        public readonly string Name;

        private StatefulConditionSet _conditionSet;
        //private List<ICondition> _completeConditions;
        private MilestoneData _data;

        protected Milestone()
        {
        }
        
        protected Milestone(MilestoneData data)
        {
            _data = data;
        }
        
        protected Milestone(string name)
        {
            Name = name;
        }

        public void AddCondition(StatefulCondition condition)
        {
            _conditionSet = new StatefulConditionSet(condition, StatefulConditionSet.Mode.All);
            //_conditionSet.(condition);
            //_completeConditions.Add(condition);
            //condition.Changed += HandleConditionChanged;
        }

        private void HandleConditionChanged()
        {
            CheckAllConditionsMet();
        }

        protected bool CheckAllConditionsMet()
        {
            if (_isComplete)
            {
                return true;
            }
            if (_conditionSet.isMet())
            {
                _isComplete = true;
            }
            return _isComplete;
        }
    }
}