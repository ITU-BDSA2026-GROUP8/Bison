namespace Bison.TestInfrastructure;

using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

public sealed class CsvDbServiceFixture : IAsyncLifetime
{
    private Process? _serviceProcess;
    private string? _temporaryDirectory;
    private string? _previousServiceUrl;
    private bool _environmentVariableSet;

    public string BaseUrl { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        try
        {
            var repositoryRoot = FindRepositoryRoot();
            var serviceProject = Path.Combine(repositoryRoot, "src/Bison.CSVDBService/Bison.CSVDBService.csproj");
            var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent?.Name ?? "Debug";
            var serviceAssembly = Path.Combine(repositoryRoot, "src/Bison.CSVDBService/bin", configuration, "net8.0/Bison.CSVDBService.dll");

            var port = GetAvailablePort();
            BaseUrl = $"http://127.0.0.1:{port}";

            _temporaryDirectory = Path.Combine(Path.GetTempPath(), $"bison-tests-{Guid.NewGuid():N}");
            var workingDirectory = Path.Combine(_temporaryDirectory, "src/Bison.CSVDBService");
            var simpleDbDirectory = Path.Combine(_temporaryDirectory, "SimpleDB");
            Directory.CreateDirectory(workingDirectory);
            Directory.CreateDirectory(simpleDbDirectory);

            foreach (var fileName in new[]
            {
                "bison_observe_cli_db.csv",
                "bison_comment_cli_db.csv",
                "Taxons.csv",
                "bison_simpleTaxon.csv"
            })
            {
                File.WriteAllText(Path.Combine(simpleDbDirectory, fileName), string.Empty);
            }

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
            };
            startInfo.ArgumentList.Add(serviceAssembly);
            startInfo.Environment["ASPNETCORE_URLS"] = BaseUrl;
            startInfo.Environment["BISON_SIMPLEDB_PATH"] = simpleDbDirectory;

            _serviceProcess = Process.Start(startInfo);

            using var client = new HttpClient { BaseAddress = new Uri(BaseUrl), Timeout = TimeSpan.FromSeconds(1) };
            var startupDeadline = DateTime.UtcNow.AddSeconds(30);
            while (DateTime.UtcNow < startupDeadline)
            {
                if (_serviceProcess.HasExited)
                {
                    throw new InvalidOperationException("Bison.CSVDBService has closed.");
                }

                try
                {
                    using var response = await client.GetAsync("observations");
                    if (response.IsSuccessStatusCode)
                    {
                        _previousServiceUrl = Environment.GetEnvironmentVariable("BISON_CSVDB_URL");
                        Environment.SetEnvironmentVariable("BISON_CSVDB_URL", BaseUrl);
                        _environmentVariableSet = true;
                        return;
                    }
                }
                catch (HttpRequestException)
                {
                }
                catch (TaskCanceledException)
                {
                }
                await Task.Delay(100);
            }

            throw new TimeoutException("Bison.CSVDBService did complete within 30 seconds");
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        if (_environmentVariableSet)
        {
            Environment.SetEnvironmentVariable("BISON_CSVDB_URL", _previousServiceUrl);
            _environmentVariableSet = false;
        }

        if (_serviceProcess is not null)
        {
            if (!_serviceProcess.HasExited)
            {
                _serviceProcess.Kill(entireProcessTree: true);
                await _serviceProcess.WaitForExitAsync();
            }

            _serviceProcess.Dispose();
            _serviceProcess = null;
        }

        if (_temporaryDirectory is not null)
        {
            Directory.Delete(_temporaryDirectory, recursive: true);
            _temporaryDirectory = null;
        }
    }
    private static int GetAvailablePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "BisonSolution.sln")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not find BisonSolution.sln");
    }
}