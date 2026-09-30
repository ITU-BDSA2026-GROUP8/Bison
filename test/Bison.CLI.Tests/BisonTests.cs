using System.Globalization;
using Bison.CLI;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bison.TestInfrastructure;

namespace Bison.CLI.Tests;

public class BisonTests : IClassFixture<CsvDbServiceFixture>
{
    private readonly CsvDbServiceFixture _service;

    public BisonTests(CsvDbServiceFixture service)
    {
        _service = service;
    }

    [Fact]
    public async Task CommentThatReferenceNonExistingObservation()
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(_service.BaseUrl);

        await UserInterface.StoreObservation("message", "somewhere", client);
        await UserInterface.StoreComment("", 99, client);

        var comments = await client.GetFromJsonAsync<List<Comment>>("comments?id=99");
        Assert.NotNull(comments);
        Assert.Empty(comments);
    }
    [Fact]
    public void UNIXTimeStampConversion()
    {
        var timenumber = DateTimeOffset.FromUnixTimeSeconds(1789070861);

        var timeinput = timenumber.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture);

        var timeConstant = "09/10/26 20:07:41";

        Assert.Equal(timeConstant,timeinput);
    }

    [Fact]
    public async Task ObserveAddsAPost()
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(_service.BaseUrl);

        var observationsBefore = await client.GetFromJsonAsync<List<Observation>>("observations");
        Assert.NotNull(observationsBefore);

        await UserInterface.StoreObservation("There is a test at ITU!", "ITU", client);

        var observationsAfter = await client.GetFromJsonAsync<List<Observation>>("observations");
        Assert.NotNull(observationsAfter);
        Assert.Equal(observationsBefore.Count + 1, observationsAfter.Count);
        Assert.Contains(observationsAfter, observation => observation.Message == "There is a test at ITU!");
    }
}