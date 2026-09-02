namespace Bison.CLI;

using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper;


public class Program
{
    public record Cheep([property: Index(0)] string Author, [property: Index(1)] string Message, [property: Index(2)] long Timestamp);
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "read")
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

        if (args[0] == "observe" && args.Length >= 2)
        {
            var author = Environment.UserName;
            var message = args[1];
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var csvLine = $"{author},\"{message}\",{time}";
            File.AppendAllText("bison_observe_cli_db.csv", csvLine + Environment.NewLine);
        }
    }
}