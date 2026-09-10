namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.VisualBasic;

public sealed class CSVDataBase<T> : IDatabaseRepository<T>
{
    IEnumerable<T> Comments;
    IEnumerable<T> Observations;

    CsvConfiguration Config;

    string Peter = "..//SimpleDB//bison_observe_cli_db.csv";
    string Peter2 = "..//SimpleDB//bison_comment_cli_db.csv";

    CSVDataBase()
    {
        this.Config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        using (var reader = new StreamReader(this.Peter))
        using (var csv = new CsvReader(reader, this.Config)) this.Observations = csv.GetRecords<T>();

        using (var reader = new StreamReader(this.Peter2))
        using (var csv = new CsvReader(reader, this.Config)) this.Comments = csv.GetRecords<T>();        
    }

    public void Store(T record)
    {
        using var writer = new StreamWriter(this.Peter, true);

        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }
    public void StoreComment(T record)
    {
        using var writer = new StreamWriter(this.Peter2, true);

        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        using (var reader = new StreamReader(this.Peter))
        using (var csv = new CsvReader(reader, this.Config))
        {
            var records = csv.GetRecords<T>().ToList<T>();
            return records;

        }
    }
    public IEnumerable<T> CommentsRead(int? limit = null)
    {
        using (var reader = new StreamReader(this.Peter2))
        using (var csv = new CsvReader(reader, this.Config))
        {
            var records = csv.GetRecords<T>().ToList<T>();
            return records;
        }
    }

    public int GetCount()
    {
        using var reader = new StreamReader(this.Peter);
        using var csv = new CsvReader(reader, this.Config);

        return csv.GetRecords<T>().Count();
    }
}