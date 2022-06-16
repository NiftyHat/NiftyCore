namespace UnityUtils
{
    public interface ISingleton<out TImplementation>
    {
        public abstract TImplementation Instance();
    }
}