using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;


public class ChatDBContext : DbContext
{

public DbSet<Observation> Observations { get; set; }
public DbSet<Author> Authors { get; set; }
public DbSet<Proposal> Proposals { get; set; }
public DbSet<Taxon> taxons { get; set; }
public DbSet<Comment> Comments { get; set; }

 public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options)
    {
        

    }
    
        

    

}

