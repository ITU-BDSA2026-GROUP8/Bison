namespace Bison.Post.Tests;
using Bison.Core.DTOs;
using Bison.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Bison.Razor;
using Bison.Core.Models;

public class PostUnitTest
{

    [Fact]
    public void StoreObservation_WithSqlInjection_IsStoredAsPlainText()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<BisonDBContext>().UseSqlite(connection).Options;
        using var context = new BisonDBContext(options);
        context.Database.EnsureCreated();
        var repo = new PostRepository(context);

        var malicious = "'; DROP TABLE Observations; --";
        repo.StoreObservation("hacker", malicious, "Aarhus");

        var posts = repo.GetObservations(1);
        Assert.Single(posts);
        Assert.Equal(malicious, posts[0].Text);
    }

    [Fact]
    public async Task PostDTOGetsMadeCorrectly()
    {
        var DTOhelper = new DTOHelper();
        var testDate = new DateTime(2004,9,26);
        var testAuthor = new Author();
        testAuthor.Id = "1";
        testAuthor.UserName = "Jens";
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
    [Fact]
    public async Task CommentDTOGetsMadeCorrectly()
    {
        var DTOhelper = new DTOHelper();
        var testDTO = new CommentDTO();
        testDTO.Text = "This is a comment for a test";
        testDTO.Id = 1;

        var DTOMadeWithMethod = DTOhelper.createCommentDTO("This is a comment for a test",1);

        Assert.Equal(testDTO.Text, DTOMadeWithMethod.Text);
        Assert.Equal(testDTO.Id,DTOMadeWithMethod.Id);
    }
    [Fact]
    public async Task TaxonDTOGetMadeCorrectly()
    {
        var DTOhelper = new DTOHelper();
        var testDTO = new TaxonDTO();
        testDTO.TaxonId = "This is a TaxonDTO test";
        testDTO.DanishVernacularName = "Det her er et dansk navn";

        var DTOMadeWithMethod = DTOhelper.createTaxonDTO("This is a TaxonDTO test",
        "Det her er et dansk navn");

        Assert.Equal(testDTO.TaxonId, DTOMadeWithMethod.TaxonId);
        Assert.Equal(testDTO.DanishVernacularName, DTOMadeWithMethod.DanishVernacularName);
    }
}
