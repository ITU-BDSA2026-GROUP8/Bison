using Bison.Core.DTOs;

namespace Bison.Infrastructure.Services;

public interface IObservationService
{
    List<PostDTO> GetObservations(int page);
    PostDTO? GetObservation(int id);
    List<PostDTO> GetObservationsFromAuthor(string author, int page);
    void StoreObservation(string author, string message, string location);
    List<CommentDTO> GetCommentsForObservation(int id);
    TaxonDTO? GetTaxonForObservation(int Id);
}
