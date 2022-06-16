using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiftyFramework.Core.Style.Swatches
{
    public class Swatch
    {
        [Serializable]
        public struct Entry
        {
            internal Color _color;
            internal string _id;
            public Entry(Color color, string id)
            {
                _id = id;
                _color = color;
            }
        }
        
        internal static Entry MissingItem = new Entry(Color.magenta, "MISSING");
        internal List<Entry> _items;

        public Swatch(List<Entry> items)
        {
            _items = items;
        }

        public bool TryGet(int index, out Color color)
        {
            if (_items == null)
            {
                color = MissingItem._color;
            }
            
            if (index < 0)
            {
                index = 0;
            }

            if (index > _items.Count)
            {
                index %= _items.Count;
            }

            color = _items[index]._color;
            return true;
        }
    }
}