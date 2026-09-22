using System.Globalization;
using Newtonsoft.Json.Converters;
using SimpleDB;

namespace Bison.CLI.Tests;

public class BisonTests
{
    [Fact]
    public void CommentThatReferenceNonExistingObservation()
    {
        var testObservation = Path.Combine(Path.GetTempPath(), $"Bison_observe_test.csv");
        var testComment = Path.Combine(Path.GetTempPath(), $"Bison_comment_test.csv");
        
        try{    
        CSVDataBase<Observation> observations = CSVDataBase<Observation>.GetInstance(testObservation);
        CSVDataBase<Comment> comments = CSVDataBase<Comment>.GetInstance(testComment);
        var thing = new Observation("author", "message", 1, 1, "somewhere");
        observations.Store(thing);
        Interface1.StoreComment("", 99, comments, observations);

        Assert.Empty(comments.Read());
        }finally{

        File.Delete(testObservation);
        File.Delete(testComment);
        }
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
    public void ObserveIdIsTheNumberOfPosts()
    {   
        var testObservation = Path.Combine(Path.GetTempPath(), $"Bison_observe_test.csv");
        try{
        CSVDataBase<Observation> observations = CSVDataBase<Observation>.GetInstance(testObservation);
        Interface1.StoreObservation("There is a test at ITU!", "ITU", observations);
        
        var expectedNumberOfPosts = 1;
        Assert.Equal(expectedNumberOfPosts,observations.GetCount());
        Assert.True(observations.GetCount()>0);
        
        }finally{

        File.Delete(testObservation);
        }
    }
}