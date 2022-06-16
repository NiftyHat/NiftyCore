using NiftyFramework.Core;
using NiftyFramework.Core.Services;

namespace NiftyFramework.Services
{
    public class UpdateService : NiftyService
    {
        public delegate void OnIntervalDelegate(float deltaTime);

        private OnIntervalDelegate _onInterval;
        
        public UpdateService()
        {
            var unityProvider = UnityDeltaTimeUpdater.Instance();
            unityProvider.OnUpdate += delegate(float time)
            {
                _onInterval?.Invoke(time);
            };
        }

        public override void Init(OnReady onReady)
        {
            onReady();
        }

        public void Add(IUpdateable updateable)
        {
            if (updateable != null)
            {
                _onInterval += updateable.Update;
            }
           
        }

        public void Remove(IUpdateable updateable)
        {
            if (updateable != null)
            {
                _onInterval -= updateable.Update;
            }
        }
        
    }
}