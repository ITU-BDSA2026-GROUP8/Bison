using Bison.Core.DTOs;

namespace Bison.Core.Services;

// Returns the observations whose taxon has the given Danish name or is a subtaxon of it.
public interface IObservationFilterService
{
    List<PostDTO> GetObservationsByTaxon(string taxonName);
}

