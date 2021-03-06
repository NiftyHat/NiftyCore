namespace NiftyFramework.UI
{
    public interface IView
    {
        public void Clear();
    }
    
    public interface IView<in TData> : IView
    {
        public void Set(TData viewData);
    }
    
    public interface IView<in TData, in TData2> : IView
    {
        public void Set(TData viewData, TData2 viewData2);
    }
    
    public interface IView<in TData, in TData2, in TData3>  : IView
    {
        public void Set(TData data, TData2 data2, TData3 data3);
    }
}