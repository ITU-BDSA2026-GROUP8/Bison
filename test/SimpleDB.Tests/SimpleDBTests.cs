using Bison.CLI;
using SimpleDB;

namespace SimpleDB.Tests;

public class SimpleDTests
{
    [Fact]
    public void EntryCanBeRecieved()
    {
        var testObservation = Path.Combine(Path.GetTempPath(), $"Bison_observe_test.csv");
        var testComment = Path.Combine(Path.GetTempPath(), $"Bison_comment_test.csv");

        try
        {
            CSVDataBase<Observation> observations = CSVDataBase<Observation>.GetInstance(testObservation);
            CSVDataBase<Comment> comments = CSVDataBase<Comment>.GetInstance(testComment);
            var case1 = new Observation("noget", "f", 1, 1, "somewhere");
            var case2 = new Observation("andet", "a", 2, 2, "somewhere");
            var case3 = new Observation("her", "g", 3, 3, "somewhere");
            
            observations.Store(case1);
            observations.Store(case2);
            observations.Store(case3);
            comments.Store(new Comment("damn", 1));

            foreach (Comment comment in comments.Read())
            {
                if (comment.Id == 1)
                {
                    Assert.Equal("damn", comment.Message);
                }
            }

            Observation[] liste = observations.Read().ToList().ToArray();

            Assert.Equal("noget", liste[0].Author);
            Assert.Equal("noget", liste[0].Author);

        }
        finally
        {

            File.Delete(testObservation);
            File.Delete(testComment);
        }

    }
}