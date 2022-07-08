using NiftyFramework.Core.Data;
using UnityEngine;

namespace NiftyFramework.UI.Unity
{
    public class MonoVisibleWithData : MonoBehaviour, IView<IOptional>, IView<object>
    {
        private bool? _defaultActiveState;
        
        public void Set(IOptional data)
        {
            _defaultActiveState ??= gameObject.activeInHierarchy;
            gameObject.SetActive(data.Enabled);
        }

        public void Set(object data)
        {
            _defaultActiveState ??= gameObject.activeInHierarchy;
            gameObject.SetActive(data != null);
        }

        public void Clear()
        {
            if (_defaultActiveState.HasValue)
            {
                gameObject.SetActive(_defaultActiveState.Value);
            }
        }
    }
}