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
        var postAuthor = _context.Authors.FirstOrDefault(candidate => candidate.Name == author);
        if (postAuthor is null)
        {
            postAuthor = new Author { Name = author, Email = string.Empty };
            _context.Authors.Add(postAuthor);
        }
        _context.Observations.Add(new Bison.Razor.Models.Observation
        {
            Author = postAuthor,
            Text = message,
            Timestamp = DateTime.UtcNow,
            Location = location
        });
        _context.SaveChanges();
    }
}
public class CommentService
{
    private readonly CSVDataBase<SimpleDB.Comment> _commentDb;
    public CommentService(CSVDataBase<SimpleDB.Comment> commentDb){_commentDb = commentDb;}
    public List<SimpleDB.Comment> GetCommentsForObservation(int id){return _commentDb.Read().Where(comment => comment.Id == id).ToList();}
}