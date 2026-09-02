namespace Bison.CLI;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;

public interface Interface1
{
    static void PrintObservations()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader("bison_observe_cli_db.csv")) using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Cheep>();
                foreach(var record in records)
                {
                    var time = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
                    Console.WriteLine($"{record.Author} @ {time.ToString ("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Message}");
                }
            }
    }
}
