using System.Collections.Generic;

namespace Level
{
    public class PlayerLevel : LevelSet
    {
        public static List<(int index, long exp)> SAMPLE = new List<(int index, long exp)>()
        {
            (1, 0), (2, 100), (3, 200), (4,222), (5,343)
        };

        private IEntry _current;
        public IEntry Current => _current;
        public long Exp;
        
        public PlayerLevel() : base(SAMPLE)
        {
            _current = First;
        }

        public IEntry AddExp(long amount, out List<IEntry> newLevels)
        {
            Exp += amount;
            if (_current.Includes(Exp))
            {
                return _current;
            }
        }
    }
    public class LevelSet
    {
        public interface IEntry
        {
            public int LevelIndex { get; }
            public long Exp { get; }
            public long ExpTotalStart { get; }
            public long ExpTotalEnd { get; }

            public bool Includes(long expValue);
            public IEntry Get(int levelIndex);
            public IEntry Get(long expTotal);
            public List<IEntry> Get(long minExp, long maxExp);
            public IEntry Next();
            public IEntry Prev();
        }
        
        public class Entry : IEntry
        {
            private int _levelIndex;
            private long _exp;
            private long _expTotal;
            internal IEntry _next;
            internal IEntry _prev;

            public int LevelIndex => _levelIndex;

            public long Exp => _exp;

            public long ExpTotalStart => _expTotal;
            public long ExpTotalEnd => _expTotal + _exp - 1;

            public Entry(int levelIndex, long exp, long expTotal)
            {
                _levelIndex = levelIndex;
                _exp = exp;
                _expTotal = expTotal;
            }

            public bool Includes(long expValue)
            {
                return expValue > ExpTotalStart && expValue < ExpTotalEnd;
            }

            public IEntry Get(int levelIndex)
            {
                if (levelIndex <= 0)
                {
                    return null;
                }
                if (levelIndex == LevelIndex)
                {
                    return this;
                }
                if (levelIndex < LevelIndex)
                {
                    var item = Prev();
                    while (item != null)
                    {
                        if (item.LevelIndex == levelIndex)
                        {
                            return item;
                        }
                        item = item.Prev();
                    }
                }
                if (levelIndex > LevelIndex)
                {
                    IEntry item = Next();
                    while (item != null)
                    {
                        if (item.LevelIndex == levelIndex)
                        {
                            return item;
                        }
                        item = item.Next();
                    }
                }
                return null;
            }

            public IEntry Get(long expAmount)
            {
                
                if (expAmount < ExpTotalStart)
                {
                    IEntry item = Prev();
                    while (item != null)
                    {
                        //keep looking backwards until there's a item that requires a low enough total exp
                        if (expAmount < item.ExpTotalStart)
                        {
                            return item;
                        }
                        item = item.Prev();
                    }
                }
                else
                {
                    IEntry item = Next();
                    while (item != null)
                    {
                        //the level we can afford will be before the first one we can't afford. 
                        if (expAmount < item.ExpTotalStart)
                        {
                            return item.Prev();
                        }
                        item = item.Next();
                    }
                }
                return null;
            }

            public List<IEntry> Get(long minExp, long maxExp)
            {
                IEntry item = Get(minExp);
                List<IEntry> results = new List<IEntry>();
                while (item != null)
                {
                    results.Add(item);
                    item = item.Next();
                    if (item.ExpTotalStart > maxExp)
                    {
                        break;
                    }
                }
                return results;
            }

            public IEntry Next()
            {
                return _next;
            }

            public IEntry Prev()
            {
                return _prev;
            }
        }

        public readonly IEntry First;
        public readonly IEntry Last;
        private IEntry _cache;

        public LevelSet(IList<(int, long)> orderData)
        {
            long totalExp = 0;
            Entry prev = null;
            for (int i = 0; i < orderData.Count; i++)
            {
                (int, long) data = orderData[i];
                totalExp += data.Item2;
                Entry entry = new Entry(data.Item1, data.Item2, totalExp);
                if (First == null)
                {
                    First = entry;
                }
                if (prev != null)
                {
                    entry._prev = prev;
                    prev._next = entry;
                }
                prev = entry;
            }
            Last = prev;
        }
    }
}