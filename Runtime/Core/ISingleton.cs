namespace UnityUtils
{
    public interface ISingleton<out TImplementation>
    {
        TImplementation Instance();
    }
}