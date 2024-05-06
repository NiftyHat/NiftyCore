using UnityEngine.UIElements;

namespace RuntimeProperties
{
    public class RuntimePropertyListViewItem : VisualElement
    {
        protected VisualElement _view;

        public RuntimePropertyListViewItem()
        {
            Add(new Label("RuntimePropertyListViewItem"));
        }
        
        public void Set(IRuntimePropertyBinder binder)
        {
            var view = binder.GetView();
            if (view != _view)
            {
                Clear();
                Add(view);
                _view = view;
            }
        }
    }
}