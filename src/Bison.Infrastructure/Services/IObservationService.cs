using Bison.Core.DTOs;

namespace Bison.Infrastructure.Services;

public interface IObservationService
{
    public List<CommentDTO> GetCommentsFromAuthor(string Author);

    List<PostDTO> GetObservations(int page);
    PostDTO? GetObservation(int id);
    List<PostDTO> GetObservationsFromAuthor(string author, int page);
    void StoreObservation(string author, string message, string location, string? imageUrl, string? imageLicense);
    List<CommentDTO> GetCommentsForObservation(int id);
    TaxonDTO? GetTaxonForObservation(int Id);
    void StoreComment(string author, string message, int observationId);
    void EditComment(string author, string message, int commentId);
    void StoreProposal(int id, string name);
    public void DeleteObservationsFromAuthor(String Author);
    public void DeleteCommentsFromAuthor(string author);
    public void DeleteAuthor(string author);
}
