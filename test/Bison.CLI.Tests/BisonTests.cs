using System.Globalization;
using Newtonsoft.Json.Converters;
using SimpleDB;

namespace Bison.CLI.Tests;

public class BisonTests
{
    [Fact]
    public void CommentThatReferenceNonExistingObservation()
    {
        var testObservation = "./Test_Bison_observe_cli_db.csv";
        var testComment = "./Test_Bison_comment_cli_db.csv";
        
            
        CSVDataBase<Observation> observations = new CSVDataBase<Observation>(testObservation);
        CSVDataBase<Comment> comments = new CSVDataBase<Comment>(testComment);
        var thing = new Observation("","",1,1);
        observations.Store(thing);
        Interface1.StoreComment("",99, comments,observations);

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
}