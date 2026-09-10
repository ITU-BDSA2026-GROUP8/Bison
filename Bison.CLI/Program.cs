namespace Bison.CLI;

using System.CommandLine;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper;
using System.Runtime.CompilerServices;
using SimpleDB;

public class Program
{   static async Task<int> Main(string[] args)
    {
        try
        {
            return await Interface1.readCommands(args);
        }catch(ArgumentNullException)
        {
            Console.WriteLine("Argument can't be null!");
            return 0;
        }

    }
}

public record Cheep([property: Index(0)] string Author, [property: Index(1)] string Message, [property: Index(2)] long Timestamp,[property: Index(3)] int Id);
public record Comment([property: Index(0)] string Message, [property: Index(1)] int Id);