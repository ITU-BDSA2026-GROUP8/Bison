namespace Bison.Razor.Models;
public abstract class Post
{
    public string text { get; set; }

    public DateTime timestamp { get; set; }

    public Author author { get; set; }
}
