using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;
using SimpleDB;


public interface IObservationService
{
    public List<PostDTO> GetObservations(int page);

    public PostDTO? GetObservation(int id);

    public List<PostDTO> GetObservationsFromAuthor(string author, int page);

    public void StoreObservation(string author, string message, string location);

    public List<CommentDTO> GetCommentsForObservation(int id);
    public TaxonDTO GetTaxonForObservation(int Id);
}
public class ObservationService : IObservationService
{
    private readonly IPostRepository _repo;

    public ObservationService(IPostRepository postRepository) { _repo = postRepository; }

    public List<PostDTO> GetObservations(int page)
    {
        return _repo.GetObservations(page);
    }

    public PostDTO? GetObservation(int id)
    {
        return _repo.GetObservation(id);
    }

    public List<PostDTO> GetObservationsFromAuthor(string author, int page)
    {
        return _repo.GetObservationsFromAuthor(author, page);
    }

    public void StoreObservation(string author, string message, string location)
    {
        _repo.StoreObservation(author, message, location);
    }

    public List<CommentDTO> GetCommentsForObservation(int id)
    {
        return _repo.GetCommentsForObservation(id);
    }

    public TaxonDTO GetTaxonForObservation(int Id)
    {
        return _repo.GetTaxonForObservation(Id);
    }
}