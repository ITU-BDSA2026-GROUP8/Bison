namespace Bison.CLI;

using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using SimpleDB;
using System.CommandLine;


public interface Interface1
{

    static async Task<int> readCommands(string[] args)
    {
        CSVDataBase<Cheep> cheeps = new CSVDataBase<Cheep>();

        var readCommand = new Command("read", "Read messages from the CSV file");
        readCommand.SetHandler(() => PrintObservations(cheeps));

        var messageArgument = new Argument<string>("message");
        var observeCommand = new Command("observe", "Observe messages and write to the CSV file") { messageArgument };
        observeCommand.SetHandler((string message) => { StoreObservation(message, cheeps); }, messageArgument);

        var rootCommand = new RootCommand("Bison Observe CLI");
        rootCommand.Add(readCommand);
        rootCommand.Add(observeCommand);

        return await rootCommand.InvokeAsync(args);
    }

    static void StoreObservation(string message, CSVDataBase<Cheep> cheeps)
    {
        var author = Environment.UserName;
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var cheep = new Cheep(author, message, time);
        cheeps.Store(cheep);
    }
    static void PrintObservations(CSVDataBase<Cheep> cheeps)
    {
        foreach (var record in cheeps.Read())
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Message}");
        }
    }
}
