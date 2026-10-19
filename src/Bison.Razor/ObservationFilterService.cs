using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;


// Service that filters observations by taxon.
// The Razor pages use this instead of calling the verified Dafny code directly.
public interface IObservationFilterService
{
    // Returns the observations whose taxon is the given taxon or one of its subtaxons.
    // Returns an empty list if the taxon id is unknown.
    List<Observation> FilterByTaxon(string taxonId, List<Observation> observations);
    
}

public class ObservationFilterService : IObservationFilterService
{
    private readonly BisonDBContext _context;

    public ObservationFilterService(BisonDBContext context)
    {
        _context = context;
    }

    public List<Observation> FilterByTaxon(string taxonId, List<Observation> observations)
    {
        // Look up the taxon to filter by
        var root = _context.taxons.Find(taxonId);

        // Unknown taxon: nothing can match
        if (root == null) 
        {
            return new List<Observation>();
        }  
        
        // The actual filtering is done by the verified (Dafny-generated) code
        return ObservationFilter.Filter(root, observations);
    }
}