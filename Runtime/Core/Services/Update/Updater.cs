using System;
using System.Collections.Generic;

namespace NiftyFramework.Core.Update
{
    public class Updater : ISubSystem, IUpdateable
    {
        public HashSet<IUpdateable> _items;

        public void Init(SubSystemStateUpdate onComplete)
        {
            onComplete(this);
        }

        public void Add(IUpdateable item)
        {
            _items.Add(item);
        }
        
        public void Remove(IUpdateable item)
        {
            _items.Remove(item);
        }

        public event Updated OnUpdated;

        public void Update(float tick = 1)
        {
            foreach (var item in _items)
            {
                item.Update(tick);
            }
            OnUpdated?.Invoke();
        }

        public void Dispose(Action onComplete)
        {
            _items.Clear();
        }
    }
}