namespace end_to_end.Tests;

using System.Globalization;
using SimpleDB;
using Bison.CLI;
using Bison.CSVDBService;

public class EndTests
{

    CSVDataBase<Bison.CLI.Observation> observations = CSVDataBase<Bison.CLI.Observation>.GetInstance("..//SimpleDB//bison_observe_cli_db.csv");
    CSVDataBase<Bison.CLI.Comment> comments = CSVDataBase<Bison.CLI.Comment>.GetInstance("..//SimpleDB//bison_comment_cli_db.csv");

    [Fact]
    public async Task FirstEndToEnd()
    {
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task ReadTest()
    {
        string[] input = { "read" };
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task ObserveTest()
    {
        string[] input = { "observe", "noget her", "i hjemmet" };
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task CommentTest()
    {
        string[] input = { "comment", "a new comment", "1" };
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task DiscussionTest()
    {
        string[] input = { "discussion", "1" };
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
        File.Delete("..//SimpleDB//bison_observe_cli_db.csv");
        File.Delete("..//SimpleDB//bison_comment_cli_db.csv");
    }
}
