namespace Bison.CLI;

using System.CommandLine;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper;
using System.Runtime.CompilerServices;
using SimpleDB;
using System.ComponentModel;

public class Program
{   public static async Task<int> Main(string[] args)
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

public record Cheep();

public record Comment([property: Index(0)] string Message, [property: Index(1)] int Id) : Cheep;
public record Taxon([property:Index(0)]string taxonID,[property:Index(2)]string parentID,[property:Index(3)]string acceptedName,[property:Index(4)]string taxonomicStatus,[property:Index(5)]string taxonRank,[property:Index(6)]string ScientificName,[property:Index(7)]string ScientificNameAuthorship,[property:Index(8)]String language,[property:Index(9)]string vernaculareName,[property:Index(10)]string merged) : Cheep;
public record simpleTaxon([property:Index(0)]string ID,[property:Index(1)]string parentID,[property:Index(2)]string Rank, [property:Index(3)]string vernaculareName,[property:Index(4)]List<simpleTaxon> children):Cheep; 
public record Observation([property: Index(0)] string Author, [property: Index(1)] string Message, [property: Index(2)] long Timestamp,[property: Index(3)] int Id, [property: Index(4)] string Location) : Cheep;
