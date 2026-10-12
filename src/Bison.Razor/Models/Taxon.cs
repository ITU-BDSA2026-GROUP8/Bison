namespace Bison.Razor.Models;
public class Taxon
{
    public string TaxonId { get; set; }

    public string DanishVernacularName { get; set; }    

    public Taxon? parent { get; set; }

    public List<Taxon> children { get; set; } = new();

public bool IsSubTaxon(Taxon ancestor)
{
    Taxon? current = this;
    while (current != null)
    {
        if (current.TaxonId == ancestor.TaxonId)
        {
            return true;
        }
        current = current.parent;
    }
    return false;
}

}

