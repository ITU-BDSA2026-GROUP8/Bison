
using System.IO.Pipes;
using Bison.Core.DTOs;
using Bison.Core.Models;
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
            var temp = helper.createPostDTO(observation.Text, observation.Author,
            observation.Timestamp, observation.Id, observation.ImageUrl, observation.ImageLicense);
            DTOList.Add(temp);
        }

        return DTOList;
    }
    public List<CommentDTO> GetCommentsFromAuthor(string author)
    {
        var commentDTOs = new List<CommentDTO>();
        var com = _context.Comments
        .Include(comment => comment.Author)
        .Where(comment => comment.Author.UserName == author);
        foreach (Comment comment in com)
        {
            Console.WriteLine(comment.Author.UserName.ToString());
            var temp = helper.createCommentDTO(comment.Text, comment.Id);
            commentDTOs.Add(temp);
        }
        return commentDTOs;
    }
    public void DeleteAuthor(string autho)
    {
        var auth = _context.Authors;
        var Author = _context.Authors.FirstOrDefault(candidate => candidate.UserName == autho);
        if(Author is not null) _context.Remove(Author);
        _context.SaveChanges();
    }
    public void AnonimisizeAuthor(string author)
    {
        var authors=_context.Authors;
        var obs =_context.Observations
        .Include(observation => observation.Author)
        .Where(observation => observation.Author.UserName == author);
        var coms= _context.Comments            
        .Include(comment => comment.Author)
        .Where(comment => comment.Author.UserName == author);
        Author AnonymousAuthor=new Author();
    foreach(Author author1 in authors)
        {
            if (author1.UserName == "anonymous")
            {
                AnonymousAuthor=author1;
                break;
            }
        }
        if (AnonymousAuthor.UserName== "anonymous")
        {
            AnonymousAuthor.UserName="anonymous";
        }
        foreach(Observation ob in obs)
        {
            if(ob.Author.UserName==author){
                ob.Author=AnonymousAuthor;
            }
        }
        foreach(Comment com in coms)
        {
            if(com.Author.UserName==author){
                com.Author=AnonymousAuthor;
            }
        }
        _context.SaveChanges();
    }
 
    
    public PostDTO? GetObservation(int id)
    {
        var obs = _context.Observations
           .Include(observation => observation.Author)
           .FirstOrDefault(observation => observation.Id == id);

        if (obs == null) return null;
        var temp = helper.createPostDTO(obs.Text, obs.Author, obs.Timestamp, obs.Id, obs.ImageUrl, obs.ImageLicense);
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
            var temp = helper.createPostDTO(observation.Text, observation.Author,
            observation.Timestamp, observation.Id, observation.ImageUrl, observation.ImageLicense);
            DTOList.Add(temp);
        }
        return DTOList;
    }
    public void StoreComment(string author, string message, int Oid)
    {
        var postAuthor = _context.Authors.FirstOrDefault(candidate => candidate.UserName == author);
        if (postAuthor is null)
        {
            postAuthor = new Author { UserName = author, Email = string.Empty };
            _context.Authors.Add(postAuthor);
        }
        Observation? observation = _context.Observations.FirstOrDefault(candidate => candidate.Id == Oid);
        _context.Comments.Add(new Bison.Core.Models.Comment
        {
            Observation = observation,
            Author = postAuthor,
            Text = message,
            Timestamp = DateTime.UtcNow
        }
        );
        if (postAuthor.UserName != null) _context.SaveChanges();
    }
    public void StoreObservation(string author, string message, string location, string? imageUrl, string? imageLicense)
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
            Location = location,
            ImageUrl = imageUrl,
            ImageLicense = imageLicense
        });
        _context.SaveChanges();

    }
    public void DeleteCommentsFromAuthor(string author)
    {
        var coms = _context.Comments
            .Include(comment => comment.Author)
            .Where(comment => comment.Author.UserName == author);
        foreach (Comment com in coms)
        {
            if (com.Author.UserName.ToString() == author) _context.Remove(com);
        }
        _context.SaveChanges();
    }
    public void DeleteObservationsFromAuthor(string author)
    {
        var obs = _context.Observations
            .Include(observation => observation.Author)
            .Where(observation => observation.Author.UserName == author);
        foreach (Observation ob in obs)
        {
            if (ob.Author.UserName.ToString() == author)
            {
                _context.Observations.Remove(ob);
            }
        }
        _context.SaveChanges();
    }
    public void StoreProposal(int id, string danishVernacularName)
    {
        var taxons = _context.taxons;
        Observation? observation = _context.Observations.FirstOrDefault(candidate => candidate.Id == id);
        foreach (Taxon tx in taxons)
        {
            if (tx.DanishVernacularName.ToLower() == danishVernacularName.ToLower())
            {
                observation.Taxon = tx;
                break;
            }

        }
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
            var temp = helper.createCommentDTO(comment.Text, comment.Id);
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
    public void EditComment(string author, string message, int commentId)
    {
        var postAuthor = _context.Authors.FirstOrDefault(candidate => candidate.UserName == author);
        if (postAuthor is null) return;

        Comment? com = _context.Comments.FirstOrDefault(candidate => candidate.Id == commentId && candidate.Author.UserName == postAuthor.UserName);
        if (com is null) return;

        com.Text = message;
        _context.SaveChanges();
    }
}
