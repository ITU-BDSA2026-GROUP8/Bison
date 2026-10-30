using Microsoft.AspNetCore.Identity;

namespace Bison.Razor.Models;


public class Author : IdentityUser
{
    public List<Post> Posts { get; set; } = new();
}

