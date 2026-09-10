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
        CSVDataBase<Observation> cheeps = new CSVDataBase<Observation>();
        CSVDataBase<Comment> comments = new CSVDataBase<Comment>();

        var messageArgument = new Argument<string>("message");
        var commentArgument = new Argument<string>("comment");
        var idArgument = new Argument<int>("id");

        var readCommand = new Command("read", "Read messages from the CSV file");
        readCommand.SetHandler(() => PrintObservations(cheeps));

        var observeCommand = new Command("observe", "Observe messages and write to the CSV file") { messageArgument };
        observeCommand.SetHandler((string message) => { StoreObservation(message, cheeps); }, messageArgument);

        var commentCommand = new Command("comment", "Comment on a message and write to the CSV file"){ commentArgument, idArgument };
        commentCommand.SetHandler((string comment, int id) => { StoreComment(comment, id, comments, cheeps); }, commentArgument, idArgument);

        var discussionCommand = new Command("discussion", "Read comments for a specific message ID") { idArgument };
        discussionCommand.SetHandler((int id) =>{printDiscussion(id, comments);}, idArgument);

        var rootCommand = new RootCommand("Bison Observe CLI");
        rootCommand.Add(readCommand);
        rootCommand.Add(observeCommand);
        rootCommand.Add(commentCommand);
        rootCommand.Add(discussionCommand);

        return await rootCommand.InvokeAsync(args);
    }
    static void printDiscussion(int id, CSVDataBase<Comment> comments){
        foreach(Comment comment in comments.CommentsRead()){
            if(comment.Id==id){
                Console.WriteLine(comment.Message);
            }
        }
    }
    
    static void StoreComment(string comment, int id, CSVDataBase<Comment> comments,CSVDataBase<Observation> cheeps)
    {
        if(!cheeps.Read().Any(c => c.Id == id))
        {
            Console.WriteLine($"No observation found with ID {id}. Cannot add comment.");
            return;
        }
        var commentRecord = new Comment(comment,id);
        comments.StoreComment(commentRecord);
    }
    
    static void StoreObservation(string message, CSVDataBase<Observation> cheeps)
    {
        var author = Environment.UserName;
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var id= cheeps.Read().Count() + 1;
        var cheep = new Observation(author, message, time, id);
        cheeps.Store(cheep);
    }

    static void PrintObservations(CSVDataBase<Observation> cheeps)
    {
        foreach (var record in cheeps.GetObservations())
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {time.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Message}");
        }
    }
}
