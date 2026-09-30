using SimpleDB;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var simpleDbPath = Environment.GetEnvironmentVariable("BISON_SIMPLEDB_PATH");
if (string.IsNullOrWhiteSpace(simpleDbPath))
{
    simpleDbPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SimpleDB"));
}

var observationDb = new CSVDataBase<Observation>(Path.Combine(simpleDbPath, "bison_observe_cli_db.csv"));
var commentDb = new CSVDataBase<Comment>(Path.Combine(simpleDbPath, "bison_comment_cli_db.csv"));
var taxonDb = new CSVDataBase<Taxon>(Path.Combine(simpleDbPath, "bison_Taxons_cli_db.csv"));
var simpleTaxonDb = new CSVDataBase<simpleTaxon>(Path.Combine(simpleDbPath, "bison_simpleTaxon.csv"));
var proposalsDB = new CSVDataBase<Proposal>(Path.Combine(simpleDbPath,"bison_proposal_cli_db.csv"));
app.MapPost("/observation", ([FromBody] Observation obs) =>
{
    observationDb.Store(obs);
    return Results.Ok(new { status = "stored", id = obs.Id });
});


app.MapPost("/comment", ([FromBody] Comment c) =>
{
    commentDb.Store(c);
    return Results.Ok(new { status = "stored" });
});

app.MapPost("/simpleTaxon", ([FromBody] simpleTaxon st) =>
{
    simpleTaxonDb.Store(st);
    return Results.Ok(new { status = "stored" });
});
app.MapPost("/proposal",([FromBody] Proposal p)=>
{
   proposalsDB.Store(p);
   return Results.Ok(new{ status = "stored"}); 
});
//d
// GET /observations
app.MapGet("/observations", () =>
{
    var all = observationDb.Read().ToList();
    return Results.Ok(all);
});

app.MapGet("/proposals", () =>
{
    var all = proposalsDB.Read().ToList();
    return Results.Ok(all);
});
// GET /comments?id=123
app.MapGet("/comments", (int id) =>
{
    var all = commentDb.Read().Where(c => c.Id == id).ToList();
    return Results.Ok(all);
});

app.MapGet("/taxons", () =>
{
    var all = taxonDb.Read().ToList();
    return Results.Ok(all);
});


app.MapGet("/simpleTaxons", () =>
{
    var all = simpleTaxonDb.Read().ToList();
    return Results.Ok(all);
});


//app.MapGet("/observations", () => new Observation("Peter", "I saw the heron again!", 1684229348));
app.Run();

//public record Observation(string Author, string Message, long Timestamp);


public partial class Program { }