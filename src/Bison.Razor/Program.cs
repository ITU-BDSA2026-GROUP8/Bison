using SimpleDB;
var builder = WebApplication.CreateBuilder(args);

var simpleDbPath = Environment.GetEnvironmentVariable("BISON_SIMPLEDB_PATH");
if (string.IsNullOrWhiteSpace(simpleDbPath))
{
    simpleDbPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SimpleDB"));
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton(new CSVDataBase<Observation>(Path.Combine(simpleDbPath, "bison_observe_cli_db.csv")));
builder.Services.AddSingleton(new CSVDataBase<Comment>(Path.Combine(simpleDbPath, "bison_comment_cli_db.csv")));
builder.Services.AddSingleton(new CSVDataBase<Proposal>(Path.Combine(simpleDbPath, "bison_proposal_cli_db.csv")));

builder.Services.AddSingleton<ObservationService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<ProposalService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
