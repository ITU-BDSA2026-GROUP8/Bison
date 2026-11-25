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

    [Test]
    public async Task clickingRegisterGetsYouToRegisterPage()
    {
        await Page.GotoAsync("http://localhost:5273");
        var registerLocator = Page.GetByText("Register", new () {Exact = true});
        await Expect(registerLocator).ToHaveAttributeAsync("href", "/Identity/Account/Register");
        await registerLocator.ClickAsync();
        await Expect(Page).ToHaveURLAsync("http://localhost:5273/Identity/Account/Register");
    }

    [Test]
    public async Task clickingLoginGetYouToLoginPage()
    {
        await Page.GotoAsync("http://localhost:5273");
        var loginLocator = Page.GetByText("Login", new () {Exact = true});
        await Expect(loginLocator).ToHaveAttributeAsync("href", "/Identity/Account/Login");
        await loginLocator.ClickAsync();
        await Expect(Page).ToHaveURLAsync("http://localhost:5273/Identity/Account/Login");
    }
    

    [TearDown]
    public async Task CleanUp()
    {
        _serverProcess.Kill(entireProcessTree: true);
        _serverProcess.Dispose();
    }
}