using NiftyFramework.Core.Condition;

namespace NiftyFramework.Milestones
{
    public interface IMilestoneCondition : ICondition
    {
        //public event IMilestoneCondition.DelegateConditionChanged Changed;
    }
    
    public class ConditionSet
    {
        public delegate void DelegateConditionChanged();

        public void AddCondition()
        {
            
        }
    }
}