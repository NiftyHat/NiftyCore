using UnityEngine;

namespace UnityUtils
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponent<TComponent>(this GameObject gameObject, out TComponent component) where TComponent : MonoBehaviour
        {
            component = gameObject.GetComponent<TComponent>();
            return component != null;
        }

        public static bool TrySetActive(this GameObject gameObject, bool isActive)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isActive);
                return true;
            }
            return false;
        }
        
        public static bool TrySetActive(this MonoBehaviour component, bool isActive)
        {
            if (component != null && component.gameObject != null)
            {
                component.gameObject.SetActive(isActive);
                return true;
            }
            return false;
        }
    }
}