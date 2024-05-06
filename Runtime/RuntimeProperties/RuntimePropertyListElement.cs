using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RuntimeProperties
{
    public class RuntimePropertyListElement : VisualElement
    {
        protected Object _data;
        protected ListView _listView;
        protected List<IRuntimePropertyBinder> _bindings;

        public RuntimePropertyListElement()
        {
            _listView = new ListView(_bindings, -1f, HandleMakeItem, HandleBindItem);
        }
        
        public RuntimePropertyListElement(RuntimePropertyBinding binding)
        {
            _listView = new ListView(_bindings, -1f, HandleMakeItem, HandleBindItem);
            _data = binding.Get();
        }

        private VisualElement HandleMakeItem()
        {
            return new RuntimePropertyListViewItem();
        }

        private void HandleBindItem(VisualElement view, int index)
        {
            if (view is RuntimePropertyListViewItem listItem)
            {
                listItem.Set(_bindings[index]);
            }
        }

        public void Set(object value)
        {
            _data = value;
            _bindings = RuntimeBindingUtility.GetBindings(_data);
            _listView.itemsSource = _bindings;
            _listView.RefreshItems();
        }

        public void Set(IList list, string propertyName)
        {
            _data = list;
            _bindings = RuntimeBindingUtility.GetBindings(list, propertyName);
            _listView.itemsSource = _bindings;
            _listView.RefreshItems();
        }

        public object Get()
        {
            return _data;
        }
    }
}