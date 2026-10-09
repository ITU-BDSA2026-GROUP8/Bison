using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;


public interface IPostRepository
{
    public List<PostDTO> GetObservations(int page);
    
    public PostDTO GetObservation(int id);

}

public class PostDTO
{
    public string Text {get; set;}

    public Author Author {get; set;}

    public DateTime Timestamp {get; set;}   
}

public class PostRepository : IPostRepository
{
    
    private readonly BisonDBContext _context;

    public PostRepository(BisonDBContext context) => _context = context;
    public List<PostDTO> GetObservations(int page)
    {
        var DTOList = new List<PostDTO>();

        var skip = (Math.Max(page, 1) - 1) * 32;
          var postlist = _context.Observations
            .Include(observation => observation.Author)
            .Skip(skip)
            .Take(32)
            .ToList();

        foreach (Observation observation in postlist)
        {
            var temp = new PostDTO();
            temp.Author = observation.Author;
            temp.Text = observation.Text;
            temp.Timestamp = observation.Timestamp;
            DTOList.Add(temp);
        }

        return DTOList;
    }
    public PostDTO GetObservation(int id)
    {
         var obs =_context.Observations
            .Include(observation => observation.Author)
            .FirstOrDefault(observation => observation.Id == id);
        var temp = new PostDTO();
        temp.Author = obs.Author;
        temp.Text = obs.Text;
        temp.Timestamp = obs.Timestamp;
        return temp;
    }

    


}