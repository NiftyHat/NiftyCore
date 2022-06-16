using System;
using NiftyFramework.Core.Services;
using NiftyFramework.Core.SubSystems;

namespace NiftyFramework.Core
{
    public abstract class App
    {
        public static ServiceSet<INiftyService> Services { get; private set; }
        public static Action<ServiceSet<INiftyService>> OnServicesAllocated;
        public ILoggerNiftyService LoggerNiftyService;

        public App()
        {
            Services = new ServiceSet<INiftyService>();
            OnServicesAllocated?.Invoke(Services);
        }
        
        public virtual void Init()
        {
            
        }

    }
}