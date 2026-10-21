namespace Bison.Razor.Models;
public class Observation : Post
{
    public string Location { get; set; }
    public Taxon? Taxon { get; set; }
    public List<Proposal> Proposals { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();

    
    public Taxon GetTaxon()
    {
        if (Taxon == null)
        {
            throw new Exception("Observation has no taxon");
        }
        return Taxon;
    }
}