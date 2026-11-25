using Bison.Core.DTOs;

namespace Bison.Infrastructure.Repositories;

public interface IPostRepository
{
    public List<PostDTO> GetObservations(int page);

    public PostDTO GetObservation(int id);

    public List<PostDTO> GetObservationsFromAuthor(string author, int page);

    public void StoreObservation(string author, string message, string location, string? imageUrl, string? imageLicense);
    public List<CommentDTO> GetCommentsForObservation(int Id);
    public List<CommentDTO> GetCommentsFromAuthor(string Author);
    public TaxonDTO? GetTaxonForObservation(int Id);

    public void StoreComment(string author, string message, int Oid);
    public void EditComment(string author, string message, int commentId);
    public void StoreProposal(int id,string name);
}