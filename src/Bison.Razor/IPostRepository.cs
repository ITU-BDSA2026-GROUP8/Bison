using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;


public interface IPostRepository
{
    public List<PostDTO> GetObservations(int page);

    public PostDTO GetObservation(int id);

    public List<PostDTO> GetObservationsFromAuthor(string author, int page);

    public void StoreObservation(string author, string message, string location);
    public List<CommentDTO> GetCommentsForObservation(int Id);
    public TaxonDTO? GetTaxonForObservation(int Id);
}

public class PostDTO
{
    public string Text { get; set; } = string.Empty;

    public Author Author { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public int Id { get; set; }
}
public class CommentDTO
{
    public string Text { get; set; } = string.Empty;

    public int Id { get; set; }
}
public class TaxonDTO
{
    public string? TaxonId { get; set; }

    public string? DanishVernacularName { get; set; }
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
    public PostDTO? GetObservation(int id)
    {
        var obs = _context.Observations
           .Include(observation => observation.Author)
           .FirstOrDefault(observation => observation.Id == id);

        if (obs == null) return null;
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
           .Include(observation => observation.Comments)
           .FirstOrDefault(observation => observation.Id == Id);

        if (obs == null) return DTOList;
        foreach (Comment comment in obs.Comments)
        {
            var temp = new CommentDTO();
            temp.Text = comment.Text;
            temp.Id = comment.Id;
            DTOList.Add(temp);
        }
        return DTOList;
    }

    public TaxonDTO? GetTaxonForObservation(int Id)
    {
        var obs = _context.Observations
           .Include(observation => observation.Author)
           .Include(observation => observation.Taxon)
           .FirstOrDefault(observation => observation.Id == Id);

        if (obs == null || obs.Taxon == null) return null;
        var temp = new TaxonDTO();
        temp.DanishVernacularName = obs.Taxon.DanishVernacularName;
        temp.TaxonId = obs.Taxon.TaxonId;
        return temp;
    }
}

//A Helper class containing methods to ensure that DTOs are made correctly
public class DTOHelper
{
    public PostDTO createPostDTO(string text, Author author, DateTime timeStamp, int id)
    {
        var DTO = new PostDTO();
        DTO.Author = author;
        DTO.Text = text;
        DTO.Timestamp = timeStamp;
        DTO.Id = id;
        return DTO;
    }

    public CommentDTO createCommentDTO(string text, int Id)
    {
        var CDTO = new CommentDTO();
        CDTO.Text = text;
        CDTO.Id = Id;
        return CDTO;
    }

    public TaxonDTO createTaxonDTO(string TaxonId, string DanishVernacularName)
    {
        var TDTO = new TaxonDTO();
        TDTO.TaxonId = TaxonId;
        TDTO.DanishVernacularName = DanishVernacularName;
        return TDTO;
    }
}