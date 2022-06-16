using UnityEngine;
using UnityUtils;

namespace Timeline.Data
{
    public abstract class ScriptableFactory<TImplementation> : ScriptableObject, IFactory<TImplementation> where TImplementation : new()
    {
        public TImplementation Create()
        {
            return new TImplementation();
        }
    }
}