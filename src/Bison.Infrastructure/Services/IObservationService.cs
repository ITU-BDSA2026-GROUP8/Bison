using Bison.Core.DTOs;

namespace Bison.Infrastructure.Services;

public interface IObservationService
{
    public List<CommentDTO> GetCommentsFromAuthor(string Author);

    List<PostDTO> GetObservations(int page);
    PostDTO? GetObservation(int id);
    List<PostDTO> GetObservationsFromAuthor(string author, int page);
    void StoreObservation(string author, string message, string location, string? imageUrl);
    List<CommentDTO> GetCommentsForObservation(int id);
    TaxonDTO? GetTaxonForObservation(int Id);
}
