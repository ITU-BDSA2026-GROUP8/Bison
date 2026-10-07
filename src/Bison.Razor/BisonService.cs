using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;
using SimpleDB;

public class ObservationService
{
    private readonly BisonDBContext _context;

    public ObservationService(BisonDBContext context) => _context = context;

    public List<Bison.Razor.Models.Observation> GetObservations(int page)
    {
        var skip = (Math.Max(page, 1) - 1) * 32;
        return _context.Observations
            .Include(observation => observation.Author)
            .Skip(skip)
            .Take(32)
            .ToList();
    }

    public Bison.Razor.Models.Observation? GetObservation(int id)
    {
        return _context.Observations
            .Include(observation => observation.Author)
            .FirstOrDefault(observation => observation.Id == id);
    }

    public List<Bison.Razor.Models.Observation> GetObservationsFromAuthor(string author, int page)
    {
        var skip = (Math.Max(page, 1) - 1) * 32;
        return _context.Observations
            .Include(observation => observation.Author)
            .Where(observation => observation.Author.Name == author)
            .Skip(skip)
            .Take(32)
            .ToList();
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