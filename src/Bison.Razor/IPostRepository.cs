using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;


public interface IPostRepository
{
    public List<PostDTO> GetObservations(int page);

    public PostDTO GetObservation(int id);

    public List<PostDTO> GetObservationsFromAuthor(string author, int page);

    public void StoreObservation(string author, string message, string location);
    public List<CommentDTO> GetCommentsForObservation(int Id);
    public TaxonDTO GetTaxonForObservation(int Id);
}

public class PostDTO
{
    public string Text { get; set; }

    public Author Author { get; set; }

    public DateTime Timestamp { get; set; }

    public int Id { get; set; }
}
public class CommentDTO
{
    public string Text { get; set; }

    public int Id { get; set; }
}
public class TaxonDTO
{
    public string TaxonId { get; set; }

    public string DanishVernacularName { get; set; }
}

public class PostRepository : IPostRepository
{

    private readonly BisonDBContext _context;

    public PostRepository(BisonDBContext context) => _context = context;
    public List<PostDTO> GetObservations(int page)
    {
        var DTOList = new List<PostDTO>();

        var skip = (Math.Max(page, 1) - 1) * 32;
        var obs = _context.Observations
          .Include(observation => observation.Author)
          .Skip(skip)
          .Take(32)
          .ToList();

        foreach (Observation observation in obs)
        {
            var temp = new PostDTO();
            temp.Author = observation.Author;
            temp.Text = observation.Text;
            temp.Timestamp = observation.Timestamp;
            temp.Id = observation.Id;
            DTOList.Add(temp);
        }

        return DTOList;
    }
    public PostDTO GetObservation(int id)
    {
        var obs = _context.Observations
           .Include(observation => observation.Author)
           .FirstOrDefault(observation => observation.Id == id);
        var temp = new PostDTO();
        temp.Author = obs.Author;
        temp.Text = obs.Text;
        temp.Timestamp = obs.Timestamp;
        return temp;
    }

    public List<PostDTO> GetObservationsFromAuthor(string author, int page)
    {
        var DTOList = new List<PostDTO>();
        var skip = (Math.Max(page, 1) - 1) * 32;
        var obs = _context.Observations
            .Include(observation => observation.Author)
            .Where(observation => observation.Author.Name == author)
            .Skip(skip)
            .Take(32)
            .ToList();
        foreach (Observation observation in obs)
        {
            var temp = new PostDTO();
            temp.Author = observation.Author;
            temp.Text = observation.Text;
            temp.Timestamp = observation.Timestamp;
            temp.Id = observation.Id;
            DTOList.Add(temp);
        }
        return DTOList;
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
    public List<CommentDTO> GetCommentsForObservation(int Id)
    {
        var DTOList = new List<CommentDTO>();
        var obs = _context.Observations
           .Include(observation => observation.Author)
           .FirstOrDefault(observation => observation.Id == Id);
        foreach (Comment comment in obs.Comments)
        {
            var temp = new CommentDTO();
            temp.Text = comment.Text;
            temp.Id = comment.Id;
            DTOList.Add(temp);
        }
        return DTOList;
    }

    public TaxonDTO GetTaxonForObservation(int Id)
    {
        var obs = _context.Observations
           .Include(observation => observation.Author)
           .FirstOrDefault(observation => observation.Id == Id);
        var temp = new TaxonDTO();
        try
        {
            temp.DanishVernacularName = obs.Taxon.DanishVernacularName;
            temp.TaxonId = obs.Taxon.TaxonId;
            return temp;
        }
        catch
        {
            return null;
        }
    }
}