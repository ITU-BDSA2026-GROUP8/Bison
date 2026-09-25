namespace Bison.CLI;

using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using SimpleDB;
using System.CommandLine;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

public interface Interface1
{

    static async Task<int> readCommands(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentNullException();
        }

        var baseURL = "http://localhost:5212";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);


        CSVDataBase<Taxon> taxons = new CSVDataBase<Taxon>("..//SimpleDB//bison_Taxon_cli_db.csv");
        CSVDataBase<simpleTaxon> simpleTaxons = new CSVDataBase<simpleTaxon>("..//SimpleDB//bison_simpleTaxon_cli_db.csv");
        //var databaseDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SimpleDB"));
        CSVDataBase<Observation> observations = CSVDataBase<Observation>.GetInstance("..//SimpleDB//bison_observe_cli_db.csv");
        CSVDataBase<Comment> comments = CSVDataBase<Comment>.GetInstance("..//SimpleDB//bison_comment_cli_db.csv");

        var messageArgument = new Argument<string>("message");
        var idArgument = new Argument<int>("id");
        var locationArgument = new Argument<string>("location");

        var readCommand = new Command("read", "Read messages from the CSV file");
        readCommand.SetHandler(() => PrintObservations(client));

        var initCommand = new Command("initial", "initialise the taxon data structure and writes to the CSV file");
        initCommand.SetHandler(() => StoreTaxons(client));

        var observeCommand = new Command("observe", "Observe messages and write to the CSV file") { messageArgument, locationArgument };
        observeCommand.SetHandler(async (string message, string location) => { await StoreObservation(message, location, client); }, messageArgument, locationArgument);

        var locationCommand = new Command("location", "Read messages from a specific location") { locationArgument };
        locationCommand.SetHandler(async (string location) => {await PrintObservationsByLocation(location, client); }, locationArgument);

        var commentCommand = new Command("comment", "Comment on a message and write to the CSV file") { messageArgument, idArgument };
        commentCommand.SetHandler(async (string comment, int id) => { await StoreComment(comment, id, client); }, messageArgument, idArgument);

        var discussionCommand = new Command("discussion", "Read comments for a specific message ID") { idArgument };
        discussionCommand.SetHandler(async (int id) => { await printDiscussion(id, client); }, idArgument);

        var rootCommand = new RootCommand("Bison Observe CLI");
        rootCommand.Add(readCommand);
        rootCommand.Add(observeCommand);
        rootCommand.Add(commentCommand);
        rootCommand.Add(discussionCommand);
        rootCommand.Add(initCommand);
        rootCommand.Add(locationCommand);
        return await rootCommand.InvokeAsync(args);
    }
    static async Task printDiscussion(int id, HttpClient client)
    {
        var com = await client.GetFromJsonAsync<List<Comment>>($"comments?id={id}");
        if (com is null)
        {
            Console.WriteLine($"No discussion found for observation ID {id}.");
            return;
        }

        foreach (Comment comment in com)
        {
            if (comment.Id == id)
            {
                Console.WriteLine(comment.Message);
            }
        }
    }
    static async Task StoreTaxons(HttpClient client)
    {
        var taxons = await client.GetFromJsonAsync<List<Taxon>>("taxon");

        List<simpleTaxon> simpleTaxonstoStore = new List<simpleTaxon>();
        foreach (Taxon taxon in taxons)
        {
            List<simpleTaxon> children = new List<simpleTaxon>();
            if (taxon.taxonRank == "order")
            {
                simpleTaxon st = new simpleTaxon(taxon.taxonID, taxon.parentID, taxon.taxonRank, taxon.vernaculareName, children);
                simpleTaxonstoStore.Add(st);
            }
            else
            {
                if (taxons.Any(t => t.taxonID == taxon.parentID))
                {
                    simpleTaxon st = new simpleTaxon(taxon.taxonID, taxon.parentID, taxon.taxonRank, taxon.vernaculareName, children);
                    foreach (simpleTaxon t in simpleTaxonstoStore)
                    {
                        if (t.ID == st.parentID)
                        {
                            t.children.Add(st);
                        }
                    }
                    simpleTaxonstoStore.Add(st);
                }
            }
        }
        foreach (simpleTaxon st in simpleTaxonstoStore)
        {
            await client.PostAsJsonAsync<simpleTaxon>("proposal", st);
        }
    }
    static async Task StoreComment(string comment, int id, HttpClient client)
    {
        var observations = await client.GetFromJsonAsync<List<Observation>>("observations");
        if (observations is null || !observations.Any(c => c.Id == id))
        {
            Console.WriteLine($"No observation found with ID {id}. Cannot add comment.");
            return;
        }
        var commentRecord = new Comment(comment, id);
        var com = await client.PostAsJsonAsync<Comment>("comment", commentRecord);
    }
    static async Task StoreObservation(string message, string location, HttpClient client)
    {
        var author = Environment.UserName;
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var id = 0;
        var cheep = new Observation(author, message, time, id, location);
        var ob = await client.PostAsJsonAsync<Observation>("observation", cheep);
    }

    static async Task PrintObservations(HttpClient client)
    {
        var ob = await client.GetFromJsonAsync<List<Observation>>("observations");

        foreach (Observation observation in ob)
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(observation.Timestamp);
            Console.WriteLine($"{observation.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {observation.Message}");
        }


        /*foreach (var record in observations.Read())
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Message}");
        }*/
    }


    static async Task PrintObservationsByLocation(string location, HttpClient client)
    {
        var ob = await client.GetFromJsonAsync<List<Observation>>("observations");
        foreach (var record in ob.Where(o => o.Location == location))
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Message}");
        }
    }

}