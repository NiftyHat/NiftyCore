using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NiftyFramework.Core.Providers
{
    public class CollectionProvider<TEntry>
    {
        private ObservableCollection<TEntry> _collection;
        public delegate void Changed<TValue>(TValue item);
        
        protected IEnumerable<TEntry> _value;
        public IEnumerable<TEntry> Value
        {
            get => _value;
            set => Set(value);
        }

        public CollectionProvider(ObservableCollection<TEntry> collection)
        {
            _collection = collection;
            collection.CollectionChanged += HandleCollectionChanged;
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //e.
        }


        private void Set(IEnumerable<TEntry> value)
        {
            
            //this is complicate :(
        }
    }
}