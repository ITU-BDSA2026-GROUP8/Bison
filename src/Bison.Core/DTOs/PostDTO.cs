using Bison.Core.Models;

namespace Bison.Core.DTOs;

public class PostDTO
{
    public string Text { get; set; } = string.Empty;
    public Author Author { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public int Id { get; set; }
}

