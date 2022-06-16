using System;
using System.Collections.Generic;
using NiftyFramework.Core.Services;
using NiftyFramework.Milestones;

namespace NiftyFramework
{
    public class MilestoneService<TMilestone> : NiftyService where TMilestone : Milestone
    {
        private HashSet<TMilestone> _trackedMilestones;
        
        public delegate void DelegateMilestoneComplete(TMilestone command);
        public delegate void DelegateMilestoneUnlocked(TMilestone command);

        public MilestoneService()
        {
            _trackedMilestones = new HashSet<TMilestone>();
        }

        public override void Init(OnReady onReady)
        {
            onReady();
        }

        public void AddMilestone(TMilestone milestone)
        {
            _trackedMilestones.Add(milestone);
        }

        public void SetMilestones(IEnumerable<TMilestone> milestones)
        {
            foreach (var item in milestones)
            {
                _trackedMilestones.Add(item);
            }
        }

        public TSubMilestoneType GetMilestone<TSubMilestoneType>(Predicate<TSubMilestoneType> predicate)
        {
            foreach (var item in _trackedMilestones)
            {
                if (item is TSubMilestoneType subMilestoneType && predicate(subMilestoneType))
                {
                    return subMilestoneType;
                }
            }
            return default;
        }
    }
}