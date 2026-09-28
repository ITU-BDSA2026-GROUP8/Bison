using System.Globalization;
using Newtonsoft.Json.Converters;
using SimpleDB;
using Bison.CLI;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Bison.CLI.Tests;

public class BisonTests
{
    [Fact]
    public async Task CommentThatReferenceNonExistingObservation()
    {   
        CSVDataBase<Comment> comments = CSVDataBase<Comment>.GetInstance("..//SimpleDB//bison_comment_cli.db.csv");

        var baseURL = "http://localhost:5212";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

        await UserInterface.StoreObservation("message", "somewhere", client);
        await UserInterface.StoreComment("", 99, client);

        Assert.Empty(comments.Read());
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
    public async Task ObserveIdIsTheNumberOfPosts()
    {   
        var baseURL = "http://localhost:5212";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

        try{
        CSVDataBase<Observation> observations = CSVDataBase<Observation>.GetInstance("..//SimpleDB//bison_observe_cli.db.csv");
        await UserInterface.StoreObservation("There is a test at ITU!", "ITU", client);
        
        var expectedNumberOfPosts = 13;
        var obs = await client.GetFromJsonAsync<List<Observation>>("observations");
        Assert.Equal(expectedNumberOfPosts, obs.Count + 1);
        Assert.True(obs.Capacity>0);
        
        }finally{

        }
    }
}