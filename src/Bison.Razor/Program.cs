using SimpleDB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var simpleDbPath = Environment.GetEnvironmentVariable("BISON_SIMPLEDB_PATH");
if (string.IsNullOrWhiteSpace(simpleDbPath))
{
    simpleDbPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SimpleDB"));
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton(new CSVDataBase<Comment>(Path.Combine(simpleDbPath, "bison_comment_cli_db.csv")));

builder.Services.AddScoped<IObservationService,ObservationService>();
builder.Services.AddScoped<IPostRepository,PostRepository>();
builder.Services.AddScoped<CommentService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    var sqliteDbPath = Path.Combine(builder.Environment.ContentRootPath, "Bison.db");
    connectionString = $"Data Source={sqliteDbPath}";
}

builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BisonDBContext>();
    dbContext.Database.Migrate();
    //DbInitializer.SeedDatabase(dbContext); //hvor den tager data fra 1.d uge 6
}

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
