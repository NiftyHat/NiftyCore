namespace NiftyFramework.Core.Services
{
    public interface INiftyService
    {
        void Init(NiftyService.OnReady onReady);
    }

    public abstract class NiftyService : INiftyService
    {
        public delegate void OnLoaded();
        public delegate void OnReady();

        public abstract void Init(OnReady onReady);
    }
}