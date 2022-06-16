using NiftyFramework.Core.Services;
using UnityEngine;

namespace NiftyFramework.Core.Context
{
    public class ContextService : ServiceSet<IContext>, INiftyService
    {
        public ContextService() : base()
        {
            Debug.Log("Create context service!");
        }
        
        public void Init(NiftyService.OnReady onReady)
        {
            
        }

        public static void Get<TContext>(HandleServiceGet<TContext> onGet) where TContext : class, IContext
        {
            //TODO dsaunders - cache static Get requests and invoke them after Init when App.Services will always not be null.
            if (App.Services != null)
            {
                App.Services.Resolve<ContextService>(service =>
                {
                    service.Resolve(onGet);
                });
            }
        }
    }
}