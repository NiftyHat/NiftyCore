using NiftyFramework.Core.Data;
using UnityEngine;

namespace NiftyFramework.UI.Unity
{
    public class MonoVisibleWithData : MonoBehaviour, IView<IOptional>, IView<object>
    {
        private bool? _defaultActiveState;
        
        public void Set(IOptional viewData)
        {
            _defaultActiveState = gameObject != null && gameObject.activeInHierarchy;
            gameObject.SetActive(viewData.Enabled);
        }

        public void Set(object viewData)
        {
            _defaultActiveState = gameObject != null && gameObject.activeInHierarchy;
            gameObject.SetActive(viewData != null);
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