namespace Bison.Core.Models;
public class Proposal : Post
{
    public Observation Observation { get; set; }

    public Taxon taxon { get; set; }
}

