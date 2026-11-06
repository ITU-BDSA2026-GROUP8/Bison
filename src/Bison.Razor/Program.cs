using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<IObservationService, ObservationService>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IObservationFilterService, ObservationFilterService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    var sqliteDbPath = Path.Combine(builder.Environment.ContentRootPath, "Bison.db");
    connectionString = $"Data Source={sqliteDbPath}";
}

builder.Services.AddDbContext<BisonDBContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
options.SignIn.RequireConfirmedAccount = true;
options.Password.RequireDigit=false;
options.Password.RequiredLength=6;
options.Password.RequireUppercase=false;
options.Password.RequireNonAlphanumeric=false;
options.Password.RequireLowercase=false;
})
.AddEntityFrameworkStores<BisonDBContext>() ;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BisonDBContext>();
    dbContext.Database.Migrate();
    DbInitializer.SeedDatabase(dbContext);
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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
