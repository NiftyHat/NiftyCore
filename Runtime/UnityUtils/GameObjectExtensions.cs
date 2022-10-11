using UnityEngine;

namespace NiftyFramework.UnityUtils
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponent<TComponent>(this GameObject gameObject, out TComponent component) where TComponent : MonoBehaviour
        {
            component = gameObject.GetComponent<TComponent>();
            return component != null;
        }
    }
}