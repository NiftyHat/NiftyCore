using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace NiftyFramework.Core.Style.Swatches
{
    public class SwatchData : ScriptableObject, IFactory<Swatch>
    {
        private List<Swatch.Entry> _items;
        private Swatch _instance;
        public Swatch Create()
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = new Swatch(_items);
            return _instance;
        }
    }
}