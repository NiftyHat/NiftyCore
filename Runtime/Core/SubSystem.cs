using System;

namespace NiftyFramework.Core
{
    public delegate void SubSystemStateUpdate(ISubSystem system);
    public interface ISubSystem
    {
        void Init(SubSystemStateUpdate onComplete);
        void Dispose(Action onComplete);
    }
    public abstract class SubSystem : ISubSystem
    {
        public bool IsEnabled;
        public abstract void Init(SubSystemStateUpdate onComplete);
        public abstract void Dispose(Action onComplete);
    }
}