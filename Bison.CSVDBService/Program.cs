using SimpleDB;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var observationDb = new CSVDataBase<Observation>("../src/SimpleDB/bison_observe_cli_db.csv");
var commentDb = new CSVDataBase<Comment>("../src/SimpleDB/bison_comment_cli_db.csv");

app.MapPost("/observation", ([FromBody] Observation obs) =>
{
    
    obs.Id = observationDb.GetCount() + 1;
    observationDb.Store(obs);
    return Results.Ok(new { status = "stored", id = obs.Id });
});


app.MapPost("/comment", ([FromBody] Comment c) =>
{
    commentDb.Store(c);
    return Results.Ok(new { status = "stored" });
});

// GET /observations
app.MapGet("/observations", () =>
{
    var all = observationDb.Read().ToList();
    return Results.Ok(all);
});

// GET /comments?id=123
app.MapGet("/comments", (int id) =>
{
    var all = commentDb.Read().Where(c => c.Id == id).ToList();
    return Results.Ok(all);
});


//app.MapGet("/observations", () => new Observation("Peter", "I saw the heron again!", 1684229348));
app.Run();

//public record Observation(string Author, string Message, long Timestamp);
