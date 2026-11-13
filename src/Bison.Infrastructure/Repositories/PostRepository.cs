
using Bison.Core.DTOs;
using Bison.Core.Models;
using Bison.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bison.Infrastructure.Repositories;
public class PostRepository : IPostRepository
{
    DTOHelper helper = new DTOHelper();
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
            var temp = helper.createPostDTO(observation.Text,observation.Author,
            observation.Timestamp,observation.Id);
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
        var temp = helper.createPostDTO(obs.Text,obs.Author,obs.Timestamp,obs.Id);
        return temp;
    }

    public List<PostDTO> GetObservationsFromAuthor(string author, int page)
    {
        var DTOList = new List<PostDTO>();
        var skip = (Math.Max(page, 1) - 1) * 32;
        var obs = _context.Observations
            .Include(observation => observation.Author)
            .Where(observation => observation.Author.UserName == author)
            .Skip(skip)
            .Take(32)
            .ToList();
        foreach (Observation observation in obs)
        {
            var temp = helper.createPostDTO(observation.Text,observation.Author,
            observation.Timestamp,observation.Id);
            DTOList.Add(temp);
        }
        return DTOList;
    }

    public void StoreObservation(string author, string message, string location)
    {

        var postAuthor = _context.Authors.FirstOrDefault(candidate => candidate.UserName == author);
        if (postAuthor is null)
        {
            postAuthor = new Author { UserName = author, Email = string.Empty };
            _context.Authors.Add(postAuthor);
        }
        _context.Observations.Add(new Bison.Core.Models.Observation
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
            var temp = helper.createCommentDTO(comment.Text,comment.Id);
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
        var temp = helper.createTaxonDTO(obs.Taxon.TaxonId,
        obs.Taxon.DanishVernacularName);
        return temp;
    }
}
