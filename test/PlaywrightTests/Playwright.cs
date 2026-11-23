namespace PlaywrightTests;

using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;


[TestFixture]
public class Playwright : PageTest
{
    private Process _serverProcess;
    [SetUp]
    public async Task Setup()
    {
        var projectPath = Path.GetFullPath(Path.Combine
        (AppContext.BaseDirectory, "../../../../../src/Bison.Razor"));
        _serverProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --no-build --urls http://localhost:5273",
            UseShellExecute = false,
        })!;

        using var http = new HttpClient();
        for (var i = 0; i < 60; i++)
        {
            try { await http.GetAsync("http://localhost:5273/");return;}
            catch {await Task.Delay(500);}
        }
        throw new Exception ("Server did not start in time!");
        
    }

    [Test]
    public async Task goingToLinkGetsYouToPublicTimeline()
    {
        await Page.GotoAsync("http://localhost:5273");
        await Expect(Page).ToHaveTitleAsync("Public timeline");
    }

    [TearDown]
    public async Task CleanUp()
    {
        _serverProcess.Kill(entireProcessTree: true);
        _serverProcess.Dispose();
    }
}