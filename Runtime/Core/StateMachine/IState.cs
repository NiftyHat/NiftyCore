using System;

namespace NiftyFramework.Core
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
}