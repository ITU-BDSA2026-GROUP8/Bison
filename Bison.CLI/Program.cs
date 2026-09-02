using System.Globalization;
using CsvHelper.Configuration;


public record Cheep(
    [index(0)]
    string Author,
    [index(1)]
    string Message, 
    [index(2)]
    long Timestamp
    );  
static void Main(string[] args){
    
if (args.Length > 0 && args[0] == "read")
{

   /* var lines = File.ReadAllLines("bison_observe_cli_db.csv");
    foreach (var line in lines)
    {
        var parts = line.Split(',');
        var author = parts[0];
        var message = parts[1].Trim('"');
        var time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(parts[2]));
        Console.WriteLine($"{author} @ {time.ToString ("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {message}");
    }*/

    var config = new CsvConfiguration(CultureInfo.InvariantCulture){
    HasHeaderRecord = false,
    };
    using (var reader = new StreamReader("bison_observe_cli_db.csv"))
    using (var csv = new CsvReader(reader,config))
    {
        var records = csv.GetRecords<Cheep>();
        Console.WriteLine(records);
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

