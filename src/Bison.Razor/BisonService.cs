using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;
using SimpleDB;

public class ObservationService
{
    private readonly IPostRepository _repo;

    private ObservationService(IPostRepository postRepository) {_repo = postRepository;}

    public  List<PostDTO> GetObservations(int page)
    {
        return _repo.GetObservations(page);
    }

    public PostDTO? GetObservation(int id)
    {
        return _repo.GetObservation(id);
    }

    public List<PostDTO> GetObservationsFromAuthor(string author, int page)
    {
        return _repo.GetObservationsFromAuthor(author,page);
    }

    public void StoreObservation(string author, string message, string location)
    {
        _repo.StoreObservation(author,message,location);
    }
}
public class CommentService
{
    private readonly CSVDataBase<SimpleDB.Comment> _commentDb;
    public CommentService(CSVDataBase<SimpleDB.Comment> commentDb){_commentDb = commentDb;}
    public List<SimpleDB.Comment> GetCommentsForObservation(int id){return _commentDb.Read().Where(comment => comment.Id == id).ToList();}
}