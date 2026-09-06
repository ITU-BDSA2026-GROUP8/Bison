namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;
using System.Globalization;

public sealed class CSVDataBase<T> : IDatabaseRepository<T>
{
    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };
        using var writer = new StreamWriter("..\\Bison.CLI\\bison_observe_cli_db.csv",true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
                HasHeaderRecord = false,
        };
        using (var reader = new StreamReader("..\\Bison.CLI\\bison_observe_cli_db.csv")) 
        using (var csv = new CsvReader(reader, config))
        {
             var records = csv.GetRecords<T>().ToList<T>();
             return records;
             
        }
    }
}