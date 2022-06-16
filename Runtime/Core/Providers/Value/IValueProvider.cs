namespace NiftyFramework.Core
{
    public interface IValueProvider
    {
    }
    
    public interface IValueProvider<TValue> : IValueProvider
    {
        TValue Value { get; }
    }
}