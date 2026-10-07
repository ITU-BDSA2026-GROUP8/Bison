namespace Bison.Razor.Models;
public abstract class Post
{
    public int Id { get; set; }
    public string Text { get; set; }

    public DateTime Timestamp { get; set; }

    public Author Author { get; set; }
}
