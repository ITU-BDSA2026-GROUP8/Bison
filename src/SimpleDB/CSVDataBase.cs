namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Concurrent;
using System.Globalization;

public sealed class CSVDataBase<T> : IDatabaseRepository<T>
{
    private static readonly Lazy<CSVDataBase<T>> Instances = new();
    private readonly CsvConfiguration Config;
    private readonly string FilePath;

    public CSVDataBase(string path)
    {
        this.Config = new CsvConfiguration(CultureInfo.InvariantCulture) 
        { 
            HasHeaderRecord = false, 
            MissingFieldFound = null,
        };
        this.FilePath = path;
        using var file = File.Open(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
    }

    public static CSVDataBase<T> GetInstance(string path)
    {
        var lazyInstance = new Lazy<CSVDataBase<T>>(() => new CSVDataBase<T>(path));

        return lazyInstance.Value;
    }

    public void Store(T record)
    {
        using var writer = new StreamWriter(this.FilePath, true);
        using var csv = new CsvWriter(writer, this.Config);
        csv.WriteRecord(record);
        csv.NextRecord();
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader(this.FilePath);
        using var csvReader = new CsvReader(reader, this.Config);
        var records = csvReader.GetRecords<T>();
        return records.ToList();
    }

    public int GetCount()
    {
        return this.Read().Count();
    }
}