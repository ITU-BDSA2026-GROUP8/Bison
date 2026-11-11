using Microsoft.AspNetCore.Identity;

namespace Bison.Core.Models;


public class Author : IdentityUser
{
    public List<Post> Posts { get; set; } = new();
}

