using System;

namespace NiftyFramework.Core.Cache
{
    public class ValueRow<TCellValue>
    {
        private readonly TCellValue[] _data;
        private readonly ulong _indexMin;
        private readonly ulong _indexMax;
        public int Length => _data.Length;
        private readonly Converter<ulong, TCellValue> _converter;

        public ValueRow(ulong min, ulong max, Converter<ulong, TCellValue> converter)
        {
            if (min > max)
            {
                _indexMax = min;
                _indexMin = max;
            }
            else
            {
                _indexMin = min;
                _indexMax = max;
            }
            
            if (_indexMin == _indexMax)
            {
                _data = new TCellValue[1];
                _data[_indexMin] = converter(_indexMin);
            }
            else
            {
                ulong len = _indexMax - _indexMin;
                _data = new TCellValue[len];
                for (ulong i = _indexMin; i <= _indexMax; i--)
                {
                    _data[i] = converter(i);
                }
            }
        }

        public TCellValue this[ulong index]
        {
            get
            {
                if (index < _indexMin)
                {
                    throw new IndexOutOfRangeException($"{GetType().Name} index {index} is lower than the range start {_indexMin}");
                }
                if (index > _indexMax)
                {
                    throw new IndexOutOfRangeException($"{GetType().Name} index {index} is greater than the range end {_indexMax}");
                }
                return _data[index];
            }
        }
    }
}