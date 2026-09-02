using System.CommandLine;
using System.CommandLine.Parsing;
using System.Globalization;

//Read metode
void ReadObservation()
{
     var lines = File.ReadAllLines("bison_observe_cli_db.csv");
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            var author = parts[0];
            var message = parts[1].Trim('"');
            var time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(parts[2]));
            Console.WriteLine($"{author} @ {time.ToString ("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture)}: {message}");
        }
}

//Observe metode
void ObserveMessage(string message)
{
    var author = Environment.UserName;
    var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    var csvLine = $"{author},\"{message}\",{time}";
    File.AppendAllText("bison_observe_cli_db.csv", csvLine + Environment.NewLine);
}


var readCommand = new Command("read", "Read messages from the CSV file"); 
readCommand.SetHandler(() => ReadObservation());


    var messageArgument = new Argument<string>("message");
    var observeCommand = new Command("observe", "Observe messages and write to the CSV file") {messageArgument};
    observeCommand.SetHandler(context =>
{
    var message = context.ParseResult.GetValueForArgument(messageArgument);
    ObserveMessage(message);
});
 

var rootCommand = new RootCommand("Bison Observe CLI");
rootCommand.Add(readCommand);
rootCommand.Add(observeCommand);

    
return await rootCommand.InvokeAsync(args);

