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
    static void Main(string[] args)
    {// async Task<int>
        
        Interface1.readCommands(args);
        //return await 
    }
}

public record Cheep([property: Index(0)] string Author, [property: Index(1)] string Message, [property: Index(2)] long Timestamp);