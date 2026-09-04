namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration.Attributes;

public sealed class CSVDataBase<T> : IDatabaseRepository<T>
{
    public void Store(T record)
    {
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        return; 
    }
}