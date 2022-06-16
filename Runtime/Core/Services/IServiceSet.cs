using System;

namespace NiftyFramework
{
    public interface IServiceSet<TServiceBase>
    {
        void Get<TService>(Action<TService> onGet) where TService : class, TServiceBase, new();
    }
}