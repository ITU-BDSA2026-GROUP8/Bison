
using Bison.Core.DTOs;
using Bison.Infrastructure.Repositories;

namespace Bison.Infrastructure.Services;
public class ObservationService : IObservationService
{
    private readonly IPostRepository _repo;

    public ObservationService(IPostRepository postRepository) { _repo = postRepository; }

    public List<PostDTO> GetObservations(int page)
    {
        return _repo.GetObservations(page);
    }
    public void DeleteObservationsFromAuthor(String Author)
    {
        _repo.DeleteObservationsFromAuthor(Author);
    }
    public PostDTO? GetObservation(int id)
    {
        return _repo.GetObservation(id);
    }

    public List<PostDTO> GetObservationsFromAuthor(string author, int page)
    {
        return _repo.GetObservationsFromAuthor(author, page);
    }
    public List<CommentDTO> GetCommentsFromAuthor(string Author)
    {
        return _repo.GetCommentsFromAuthor(Author);
    }

    public void StoreObservation(string author, string message, string location, string? imageUrl, string? imageLicense)
    {
        _repo.StoreObservation(author, message, location, imageUrl, imageLicense);
    }
        public void DeleteCommentsFromAuthor(string author)
    {
        _repo.DeleteCommentsFromAuthor(author);
    }


    public List<CommentDTO> GetCommentsForObservation(int id)
    {
        return _repo.GetCommentsForObservation(id);
    }
    public void DeleteAuthor(string author)
    {
        _repo.DeleteAuthor(author);
    }


    public TaxonDTO? GetTaxonForObservation(int Id)
    {
        return _repo.GetTaxonForObservation(Id);
    }
    public void StoreProposal(int id,string name)
    {
        _repo.StoreProposal(id,name);
    }
    public void StoreComment(string author,string message, int Oid)
    {
        _repo.StoreComment(author,message,Oid);
    }

    public void EditComment(string author, string message, int commentId)
    {
        _repo.EditComment(author, message, commentId);
    }
}