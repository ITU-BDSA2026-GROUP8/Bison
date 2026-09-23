namespace Bison.CLI;

using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using SimpleDB;
using System.CommandLine;
using System.Collections.Immutable;
using System.Linq;

public interface UserInterface
{

    static async Task<int> readCommands(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentNullException();
        }


        //var databaseDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SimpleDB"));
        CSVDataBase<Observation> observations = CSVDataBase<Observation>.GetInstance("..//SimpleDB//bison_observe_cli_db.csv");
        CSVDataBase<Comment> comments = CSVDataBase<Comment>.GetInstance("..//SimpleDB//bison_comment_cli_db.csv");

        var messageArgument = new Argument<string>("message");
        var idArgument = new Argument<int>("id");

        var readCommand = new Command("read", "Read messages from the CSV file");
        readCommand.SetHandler(() => PrintObservations(observations));

        var locationArgument = new Argument<string>("location");

        var observeCommand = new Command("observe", "Observe messages and write to the CSV file") { messageArgument, locationArgument };
        observeCommand.SetHandler((string message, string location) => { StoreObservation(message, location, observations); }, messageArgument, locationArgument);

        var locationCommand = new Command("location", "Read messages from a specific location") { locationArgument };
        locationCommand.SetHandler((string location) => { PrintObservationsByLocation(location, observations); }, locationArgument);

        var commentCommand = new Command("comment", "Comment on a message and write to the CSV file") { messageArgument, idArgument };
        commentCommand.SetHandler((string comment, int id) => { StoreComment(comment, id, comments, observations); }, messageArgument, idArgument);

        var discussionCommand = new Command("discussion", "Read comments for a specific message ID") { idArgument };
        discussionCommand.SetHandler((int id) => { printDiscussion(id, comments); }, idArgument);

        var rootCommand = new RootCommand("Bison Observe CLI");
        rootCommand.Add(readCommand);
        rootCommand.Add(observeCommand);
        rootCommand.Add(commentCommand);
        rootCommand.Add(discussionCommand);
        rootCommand.Add(locationCommand);
        return await rootCommand.InvokeAsync(args);
    }
    static void printDiscussion(int id, CSVDataBase<Comment> comments)
    {
        foreach (Comment comment in comments.Read())
        {
            if (comment.Id == id)
            {
                Console.WriteLine(comment.Message);
            }
        }
    }

    static void StoreComment(string comment, int id, CSVDataBase<Comment> comments, CSVDataBase<Observation> observations)
    {
        if (!observations.Read().Any(c => c.Id == id))
        {
            Console.WriteLine($"No observation found with ID {id}. Cannot add comment.");
            return;
        }
        var commentRecord = new Comment(comment, id);
        comments.Store(commentRecord);
    }

    static void StoreObservation(string message, string location, CSVDataBase<Observation> observations)
    {
        var author = Environment.UserName;
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var id = observations.GetCount() + 1;
        var cheep = new Observation(author, message, time, id, location);
        observations.Store(cheep);
    }

    static void PrintObservations(CSVDataBase<Observation> observations)
    {
        foreach (var record in observations.Read())
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Message}");
        }
    }


    static void PrintObservationsByLocation(string location, CSVDataBase<Observation> observations)
    {
        foreach (var record in observations.Read().Where(o => o.Location == location))
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Message}");
        }
    }

}