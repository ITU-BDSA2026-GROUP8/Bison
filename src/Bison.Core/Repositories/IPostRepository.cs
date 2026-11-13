using Bison.Core.DTOs;

namespace Bison.Core.Repositories;

public interface IPostRepository
{
    public List<PostDTO> GetObservations(int page);

    public PostDTO GetObservation(int id);

    public List<PostDTO> GetObservationsFromAuthor(string author, int page);

    public void StoreObservation(string author, string message, string location);
    public List<CommentDTO> GetCommentsForObservation(int Id);
    public TaxonDTO? GetTaxonForObservation(int Id);
}