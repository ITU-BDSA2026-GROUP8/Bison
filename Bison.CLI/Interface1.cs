namespace Bison.CLI;

using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using SimpleDB;
using System.CommandLine;
using System.Collections.Immutable;

public interface Interface1
{

    static async Task<int> readCommands(string[] args)
    {
        if(args.Length == 0)
        {
            throw new ArgumentNullException();
        }

        
        CSVDataBase<Cheep> cheeps = new CSVDataBase<Cheep>();
        CSVDataBase<Comment> comments = new CSVDataBase<Comment>();

        var readCommand = new Command("read", "Read messages from the CSV file");
        readCommand.SetHandler(() => PrintObservations(cheeps));

        var messageArgument = new Argument<string>("message");
        var commentArgument = new Argument<string>("comment");
        var idArgument = new Argument<int>("id");
        var discussionCommand = new Command("discussion", "Read comments for a specific message ID") { idArgument };
        discussionCommand.SetHandler((int id) =>{printDiscussion(id, comments);}, idArgument);
        var observeCommand = new Command("observe", "Observe messages and write to the CSV file") { messageArgument };
        var commentCommand = new Command("comment", "Comment on a message and write to the CSV file"){ commentArgument, idArgument };
        observeCommand.SetHandler((string message) => { StoreObservation(message, cheeps); }, messageArgument);
        commentCommand.SetHandler((string comment, int id) => { StoreComment(comment, id, comments, cheeps); }, commentArgument, idArgument);
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
    
    static void StoreComment(string comment, int id, CSVDataBase<Comment> comments,CSVDataBase<Cheep> cheeps)
    {
        if(!cheeps.Read().Any(c => c.Id == id))
        {
            Console.WriteLine($"No observation found with ID {id}. Cannot add comment.");
            return;
        }
      /*  if(comments.Read().Any(c => c.Id==id&&c.Message==comment))
        {
            Console.WriteLine("Comment already exists for this observation.");
            return;
        }*/
        var commentRecord = new Comment(comment,id);
        comments.StoreComment(commentRecord);
    }
    
    static void StoreObservation(string message, CSVDataBase<Cheep> cheeps)
    {
        var author = Environment.UserName;
        var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var id= cheeps.Read().Count() + 1;
        var cheep = new Cheep(author, message, time, id);
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
