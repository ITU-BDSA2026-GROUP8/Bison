namespace end_to_end.Tests;

using System.Globalization;
using SimpleDB;
using Bison.CLI;

public class EndTests
{
    [Fact]
    public async Task EndToEndTest()
    {
        string[] input = {"read"};
        var exitCode = await Program.Main(input);
        Assert.Equal(0, exitCode);
    }
}
