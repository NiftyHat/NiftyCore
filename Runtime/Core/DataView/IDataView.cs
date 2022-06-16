namespace NiftyFramework.DataView
{
    public interface IDataView<TData>
    {
        void Clear();
        void Set(TData data);
    }

    public interface IDataView<TData, TData2>
    {
        void Clear();
        void Set(TData data, TData2 data2);
    }

    public interface IDataView<TData, TData2, TData3>
    {
        void Clear();
        void Set(TData data, TData2 data2, TData3 data3);
    }

}