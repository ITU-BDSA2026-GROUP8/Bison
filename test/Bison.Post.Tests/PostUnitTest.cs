namespace Bison.Post.Tests;

using Bison.Razor;
using Bison.Razor.Models;

public class PostUnitTest
{

    [Fact]
    public async Task PostDTOGetsMadeCorrectly()
    {
        var DTOhelper = new DTOHelper();
        var testDate = new DateTime(2004,9,26);
        var testAuthor = new Author();
        testAuthor.AuthorId = 1;
        testAuthor.Name = "Jens";
        testAuthor.Email = "jmbe@itu.dk";
        testAuthor.Posts = new List<Post>();
        var testDTO = new PostDTO();
        testDTO.Id = 2;
        testDTO.Text = "This is a test";
        testDTO.Author = testAuthor;
        testDTO.Timestamp = testDate;

        var DTOMadeWithMethod = DTOhelper.createPostDTO("This is a test", testAuthor,new DateTime(2004,9,26),2);

        Assert.Equal(testDTO.Id, DTOMadeWithMethod.Id);
        Assert.Equal(testDTO.Author,DTOMadeWithMethod.Author);    
        Assert.Equal(testDTO.Text,DTOMadeWithMethod.Text);
        Assert.Equal(testDTO.Timestamp,DTOMadeWithMethod.Timestamp);
    }
}
