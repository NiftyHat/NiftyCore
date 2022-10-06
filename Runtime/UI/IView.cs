namespace NiftyFramework.UI
{
    public interface IView
    {
        void Clear();
    }
    
    public interface IView<in TData> : IView
    {
        void Set(TData viewData);
    }
    
    public interface IView<in TData, in TData2> : IView
    {
        void Set(TData viewData, TData2 amount);
    }
    
    public interface IView<in TData, in TData2, in TData3>  : IView
    {
        void Set(TData data, TData2 data2, TData3 data3);
    }
}