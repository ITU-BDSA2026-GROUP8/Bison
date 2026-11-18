using Bison.Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bison.Razor.Tests;

public class XssTests
{

[Fact]
    public async Task Observation_With_Script_Is_Html_Encoded()
    {
        // In-memory database instead of the real one
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {

        // Dummy GitHub OAuth values so the app can start in CI (no real login happens)
        builder.UseSetting("authentication:github:clientId", "test");
        builder.UseSetting("authentication:github:clientSecret", "test");

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<BisonDBContext>>();
                services.AddDbContext<BisonDBContext>(o => o.UseSqlite(connection));
            });
        });

        // Store a malicious observation directly in the database
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BisonDBContext>();
            context.Database.EnsureCreated();
            context.Observations.Add(new Observation
            {
                Author = new Author { UserName = "hacker", Email = "h@example.com" },
                Text = "<script>alert('xss')</script>",
                Timestamp = DateTime.UtcNow,
                Location = "Test"
            });
            context.SaveChanges();
        }

        // Fetch the user's timeline as a browser would
        var client = factory.CreateClient();
        var html = await client.GetStringAsync("/hacker");
        // The script must be shown as encoded text, not as a real script tag
        Assert.Contains("&lt;script&gt;alert(", html);
        Assert.DoesNotContain("<script>alert('xss')</script>", html);
    }
}