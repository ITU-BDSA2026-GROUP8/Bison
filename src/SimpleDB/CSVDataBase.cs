namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.VisualBasic;

public sealed class CSVDataBase<T> : IDatabaseRepository<T>
{
    CsvConfiguration Config;

    string FilePath = "..//SimpleDB//bison_observe_cli_db.csv";

    public CSVDataBase(String path)
    {
        this.Config = new CsvConfiguration(CultureInfo.InvariantCulture) 
        { 
            HasHeaderRecord = false, 
            MissingFieldFound = null,
        };
        this.FilePath = path;
    }

    public void Store(T record)
    {
        using var writer = new StreamWriter(this.FilePath, true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }

    }

    public IEnumerable<T> Read(int? limit = null)
    {   
        var Reader = new StreamReader(this.FilePath);
        var CSVReader = new CsvReader(Reader, this.Config);
        return CSVReader.GetRecords<T>().ToList();
    }
    public int GetCount()
    {
        var Reader = new StreamReader(this.FilePath);
        var CSVReader = new CsvReader(Reader, this.Config);
        return CSVReader.GetRecords<T>().Count();
    }
}