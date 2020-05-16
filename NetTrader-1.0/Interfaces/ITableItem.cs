namespace Interfaces
{
    public interface ITableItem<T>
    {
        int ColumnsCount { get; }
        T GetParams(int i);
        object GetValue(int index);
        string GetStringValue(int index);
    }
}