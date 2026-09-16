namespace end_to_end.Tests;

using System.Globalization;
using SimpleDB;

public class EndTests
{
    [Fact]
    public void UNIXTimeStampConversion()
    {
        var timenumber = DateTimeOffset.FromUnixTimeSeconds(1789070861);

        var timeinput = timenumber.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture);

        var timeConstant = "09/10/26 20:07:41";

        Assert.Equal(timeConstant,timeinput);
    }
}
