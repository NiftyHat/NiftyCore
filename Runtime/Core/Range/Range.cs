using System;

namespace NiftyFramework.Core
{
    public struct Range<TValue> where TValue : IComparable
    {
        private TValue _min;
        private TValue _max;

        public TValue Min
        {
            get => _min;
            set
            {
                if (_min.CompareTo(_max) > 0)
                {
                    throw new Exception($"[{GetType().Name}] {_min} cannot be greater than {_max}");
                }
                _min = value;
            }
        }
        
        public TValue Max
        {
            get => _max;
            set
            {
                if (_max.CompareTo(_min) > 0)
                {
                    throw new Exception($"[{GetType().Name}] {_min} cannot be greater than {_max}");
                }
                _max = value;
            }
        }

        public Range(TValue min, TValue max)
        {
            _min = min;
            _max = max;
        }

        public int GreaterThan(TValue value)
        {
            int result = value.CompareTo(_max);
            return result > 0 ? result : 0;
        }

        public int LessThan(TValue value)
        {
            int result = value.CompareTo(_min);
            return result < 0 ? result : 0;
        }

        public bool Contains(TValue value)
        {
            return value.CompareTo(_min) >= 0 && value.CompareTo(_max) <= 0;
        }

        public override string ToString()
        {
            return $"{_min}-{_max}";
        }
    }
}