namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.VisualBasic;

public sealed class CSVDataBase<T> : IDatabaseRepository<T>
{
    CsvReader CSVReader;
    CsvConfiguration Config;

    string FilePath = "..//SimpleDB//bison_observe_cli_db.csv";

    public CSVDataBase(String identity)
    {
        this.Config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false, };

        if (identity == "observation")
        {
            this.FilePath = "..//SimpleDB//bison_observe_cli_db.csv";
            var ObservationReader = new StreamReader(this.FilePath);
            this.CSVReader = new CsvReader(ObservationReader, this.Config);
        }
        else
        {
            this.FilePath = "..//SimpleDB//bison_comment_cli_db.csv";
            var CommentReader = new StreamReader(this.FilePath);
            this.CSVReader = new CsvReader(CommentReader, this.Config);
        }
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
        return this.CSVReader.GetRecords<T>().ToList();
    }
    public int GetCount()
    {
        return this.CSVReader.GetRecords<T>().Count();
    }
}