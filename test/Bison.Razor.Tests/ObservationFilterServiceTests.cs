using Bison.Razor.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Bison.Razor.Tests;

public class ObservationFilterServiceTests
{
    // Taxon tree: Aarefodede -> Hejrer -> Fiskehejre, Aarefodede -> Skarver, Aarefodede -> Tom
    // Observations: "grey heron" = Fiskehejre, "heron family" and "second heron" = Hejrer,
    // "cormorant" = Skarver, "no taxon" = none. Tom has no observations.
    private static ObservationFilterService CreateService()
    {
        // Create an in-memory SQLite database for testing
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // Make EF Core use the in-memory database
        var options = new DbContextOptionsBuilder<BisonDBContext>().UseSqlite(connection).Options;

        // Create the context and the tables
        var context = new BisonDBContext(options);
        context.Database.EnsureCreated();

        // Add test data to the in-memory database
        var root = new Taxon { TaxonId = "1", DanishVernacularName = "Aarefodede" };
        var hejrer = new Taxon { TaxonId = "2", DanishVernacularName = "Hejrer", parent = root };
        var fiskehejre = new Taxon { TaxonId = "3", DanishVernacularName = "Fiskehejre", parent = hejrer };
        var skarver = new Taxon { TaxonId = "4", DanishVernacularName = "Skarver", parent = root };
        var tom = new Taxon { TaxonId = "5", DanishVernacularName = "Tom", parent = root };
        context.taxons.AddRange(root, hejrer, fiskehejre, skarver, tom);

        // Add observations with different taxons and texts
        var author = new Author { Name = "Test", Email = "t@example.com" };
        context.Observations.AddRange(
            Obs(author, "grey heron", fiskehejre),
            Obs(author, "heron family", hejrer),
            Obs(author, "second heron", hejrer),
            Obs(author, "cormorant", skarver),
            Obs(author, "no taxon", null));
        context.SaveChanges();

        // Create the service with the in-memory context
        return new ObservationFilterService(context);
    }

    // Helper: creates an observation
    private static Observation Obs(Author a, string text, Taxon? t) =>
        new() { Author = a, Text = text, Timestamp = DateTime.UtcNow, Location = "Test", Taxon = t };

    // Helper: extracts and sorts the texts from a list of PostDTOs
    private static List<string> Texts(List<PostDTO> posts) =>
        posts.Select(p => p.Text).OrderBy(t => t).ToList();

    // Tests

    // A species taxon should only include observations of that species
    [Fact]
    public void Species_Gives_Only_That_Species() =>
        Assert.Equal(new[] { "grey heron" }, Texts(CreateService().GetObservationsByTaxon("Fiskehejre")));

    // A parent taxon should include observations of all subtaxons, but not siblings (name is case-insensitive)
    [Fact]
    public void Parent_Includes_Subtaxons_But_Not_Siblings() =>
        Assert.Equal(new[] { "grey heron", "heron family", "second heron" }, Texts(CreateService().GetObservationsByTaxon("hejrer")));

    // The top taxon should include all levels of subtaxons, but not observations without a taxon
    [Fact]
    public void Top_Taxon_Excludes_Observation_Without_Taxon() =>
        Assert.Equal(new[] { "cormorant", "grey heron", "heron family", "second heron" }, Texts(CreateService().GetObservationsByTaxon("Aarefodede")));

    // A known taxon with no observations gives an empty list (not an error)
    [Fact]
    public void Taxon_Without_Observations_Gives_Empty_List() =>
        Assert.Empty(CreateService().GetObservationsByTaxon("Tom"));

    // Unknown, empty or whitespace-only names give an empty list
    [Theory]
    [InlineData("Findes ikke")]
    [InlineData("")]
    [InlineData(" ")]
    public void Unknown_Or_Empty_Name_Gives_Empty_List(string name) =>
        Assert.Empty(CreateService().GetObservationsByTaxon(name));
}