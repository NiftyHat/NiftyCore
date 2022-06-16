using System;
using UnityEngine;

namespace NiftyFramework.Core.Style.Swatches
{
    [Serializable]
    public class SwatchReference : ISerializationCallbackReceiver
    {
        [SerializeField] protected SwatchData _data;
        [SerializeField] protected int _index;

        private Swatch _swatch;

        public bool TryGet(out Color color)
        {
            if (_swatch != null)
            {
                return _swatch.TryGet(_index, out color);
            }
            color = default;
            return false;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _swatch = _data.Create();
        }
    }
}