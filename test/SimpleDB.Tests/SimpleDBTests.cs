using Bison.CLI;
using SimpleDB;

namespace SimpleDB.Tests;

public class SimpleDTests
{
    [Fact]
    public void EntryCanBeRecieved()
    {
        try
        {
            CSVDataBase<Observation> observations = CSVDataBase<Observation>.GetInstance("..//SimpleDB//Bison_observe_test.csv");
            CSVDataBase<Comment> comments = CSVDataBase<Comment>.GetInstance("..//SimpleDB//Bison_comment_test.csv");
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
            Assert.Equal("a", liste[1].Message);
            Assert.Equal(3, liste[2].Timestamp);

            var case4 = new Observation("dom", "jajaja", 3, 4, "somewhere");
            observations.Store(case4);
            Assert.Equal(3, case4.Timestamp);
            Assert.Equal(4, case4.Id);
            Assert.Equal("jajaja", case4.Message);
        }
        finally
        {
            File.Delete("..//SimpleDB//Bison_observe_test.csv");
            File.Delete("..//SimpleDB//Bison_comment_test.csv");
        }
    }
}