using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;


public interface IPostRepository
{
    public PostDTO findPost(int id);
    
}

public class PostDTO
{
    
    public string Message {get; set;}

    public Author Author {get; set;}

    public int id {get; set;}

    public DateTime date {get; set;}
    
}

public class postRepository : IPostRepository
{
    
    private readonly BisonDBContext _context;

    public postRepository(BisonDBContext context) => _context = context;
    public async Task<PostDTO> findPost(int id)
    {
        var query = _context.Observations.Select(observation => observation.Text)
        .FirstOrDefault(observation => observation.Id == id);
        return new PostDTO();
    }
}