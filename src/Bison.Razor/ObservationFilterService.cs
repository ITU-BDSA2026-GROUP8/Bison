using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;

// Service that filters observations by taxon.
// The Razor pages use this instead of calling the verified Dafny code directly.
public interface IObservationFilterService
{
    // Returns the observations (as PostDTOs) whose taxon has the given Danish name or is a subtaxon of it.
    // Observations without a taxon are left out. Returns an empty list if no taxon has that name.
    List<PostDTO> GetObservationsByTaxon(string taxonName);
}

public class ObservationFilterService : IObservationFilterService
{
    private readonly BisonDBContext _context;

    public ObservationFilterService(BisonDBContext context)
    {
        _context = context;
    }

    public List<PostDTO> GetObservationsByTaxon(string taxonName)
    {

        if (string.IsNullOrWhiteSpace(taxonName))
        {
            return new List<PostDTO>();
        }

        // Load every taxon. This makes EF Core link each one to its Parent,
        // which IsSubTaxon needs to walk up the taxon tree.
        var taxons = _context.taxons.ToList();

        var root = taxons.FirstOrDefault(t =>
            string.Equals(t.DanishVernacularName, taxonName, StringComparison.OrdinalIgnoreCase));


        if (root == null)
        {
            return new List<PostDTO>();
        }

        // Load all observations with their authors and taxons. Observations without a taxon are left out.
        var all = _context.Observations
            .Include(o => o.Author)
            .Include(o => o.Taxon)
            .Where(o => o.Taxon != null)
            .ToList();

        // The actual filtering is done by the verified (Dafny-generated) code:
        // keeps the observations whose taxon is root or one of its subtaxons
        return ObservationFilter.Filter(root, all)
            .Select(o => new PostDTO
            {
                Author = o.Author,
                Text = o.Text,
                Timestamp = o.Timestamp,
                Id = o.Id
            })
            .ToList();
    }
}