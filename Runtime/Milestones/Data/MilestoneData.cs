using UnityEngine;
using UnityUtils;

namespace NiftyFramework.Milestones
{
    public abstract class MilestoneData : ScriptableObject, IFactory<Milestone>
    {
        public abstract Milestone Create();
    }
}