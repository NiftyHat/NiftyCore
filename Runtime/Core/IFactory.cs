using System.Collections.Generic;

namespace UnityUtils
{
    public interface IFactory<out TImplementation>
    {
        TImplementation Create();
    }

    public abstract class Factory<TImplementation> : IFactory<TImplementation> where TImplementation : new()
    {
        public TImplementation Create()
        {
            return new TImplementation();
        }
    }


    public interface IListFactory<TImplementation>
    {
       List<TImplementation> Create();
    }
}