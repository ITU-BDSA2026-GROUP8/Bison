namespace Bison.CLI;

using System.CommandLine;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper;
using System.Runtime.CompilerServices;
using SimpleDB;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        CSVDataBase<Cheep> cheeps = new CSVDataBase<Cheep>();

        var readCommand = new Command("read", "Read messages from the CSV file");
        readCommand.SetHandler(() => Interface1.PrintObservations());

        var messageArgument = new Argument<string>("message");
        var observeCommand = new Command("observe", "Observe messages and write to the CSV file") { messageArgument };
        observeCommand.SetHandler(context =>
        {
            var message = context.ParseResult.GetValueForArgument(messageArgument);
            var author = Environment.UserName;
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var cheep = new Cheep(author, message,time);
            cheeps.Store(cheep);
        });

        var rootCommand = new RootCommand("Bison Observe CLI");
        rootCommand.Add(readCommand);
        rootCommand.Add(observeCommand);

        return await rootCommand.InvokeAsync(args);
    }
}

public record Cheep([property: Index(0)] string Author, [property: Index(1)] string Message, [property: Index(2)] long Timestamp);