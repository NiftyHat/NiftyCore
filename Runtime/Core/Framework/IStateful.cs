using System;

namespace NiftyFramework.Core
{
    public interface IStateful
    {
        event Action<bool, IStateful> OnStateChanged;
    }
}