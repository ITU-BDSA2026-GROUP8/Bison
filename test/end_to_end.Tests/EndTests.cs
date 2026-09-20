namespace end_to_end.Tests;

using System.Globalization;
using SimpleDB;
using Bison.CLI;

public class EndTests
{
    [Fact]
    public async Task ReadTest()
    {
        string[] input = {"read"};
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task ObserveTest()
    {  
        string[] input = {"observe", "noget her"};
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }
    
    [Fact]
    public async Task CommentTest()
    {
        string[] input = {"comment", "a new comment", "1"};
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task DiscussionTest()
    {
        string[] input = {"discussion", "1"};
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }
}
