using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


public class BisonDBContext : IdentityDbContext<IdentityUser>{

public DbSet<Observation> Observations { get; set; }
public DbSet<Author> Authors { get; set; }
public DbSet<Proposal> Proposals { get; set; }
public DbSet<Taxon> taxons { get; set; }
public DbSet<Comment> Comments { get; set; }

 public BisonDBContext(DbContextOptions<BisonDBContext> options) : base(options)
    {
        

    }
}