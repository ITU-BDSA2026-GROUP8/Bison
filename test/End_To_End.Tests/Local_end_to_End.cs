using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace end_to_endTest;

public class Local_end_to_End
{
    /*
    [Fact]
    public async Task PostingObservationPersistsAndRedirects()
    {
        var root = FindRepositoryRoot();
        var Listener = new TcpListener(IPAddress.Loopback, 0);
        Listener.Start();
        
        var address = $"http://127.0.0.1:{((IPEndPoint)Listener.LocalEndpoint).Port}";
        Listener.Stop();

        var tempDirectory = Path.Combine(Path.GetTempPath(), $"bison-e2e-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectory);
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = root,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--no-build");
        startInfo.ArgumentList.Add("--no-restore");
        startInfo.ArgumentList.Add("--no-launch-profile");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(Path.Combine(root, "src", "Bison.Razor", "Bison.Razor.csproj"));
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        startInfo.Environment["ASPNETCORE_URLS"] = address;
        startInfo.Environment["ConnectionStrings__DefaultConnection"] =
            $"Data Source={Path.Combine(tempDirectory, "test.db")}";
        startInfo.Environment["Logging__LogLevel__Default"] = "Warning";

        using var app = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the Razor app.");
        using var client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            CookieContainer = new CookieContainer()
        }) { BaseAddress = new Uri(address) };

        try
        {
            var timeout = DateTime.UtcNow.AddSeconds(30);
            HttpResponseMessage pageResponse;
            while (true)
            {
                if (app.HasExited)
                    throw new InvalidOperationException($"The Razor app exited with code {app.ExitCode}.");
                try
                {
                    pageResponse = await client.GetAsync("/");
                    break;
                }
                catch (HttpRequestException) when (DateTime.UtcNow < timeout)
                {
                    await Task.Delay(250);
                }
            }

            using (pageResponse)
            {
                Assert.Equal(HttpStatusCode.OK, pageResponse.StatusCode);
                var page = await pageResponse.Content.ReadAsStringAsync();
                var token = Regex.Match(page, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"");
                Assert.True(token.Success, "The public page did not return an antiforgery token.");

                var message = $"Test observation {Guid.NewGuid():N}";
                var response = await client.PostAsync("/", new FormUrlEncodedContent(
                [
                    new("Message", message),
                    new("Location", "Test location"),
                    new("__RequestVerificationToken", token.Groups[1].Value)
                ]));
                using (response)
                    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

                Assert.Contains(message, await client.GetStringAsync("/Anonymous"));
            }
        }
        finally
        {
            if (!app.HasExited)
            {
                app.Kill(entireProcessTree: true);
                await app.WaitForExitAsync();
            }

            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "src", "Bison.Razor", "Bison.Razor.csproj")))
                return directory.FullName;

        throw new DirectoryNotFoundException("Could not locate the Bison repository root.");
    }*/
}
