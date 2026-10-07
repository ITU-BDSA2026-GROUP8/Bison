namespace Bison.Razor.Models;
public class Taxon
{
    public string DwcTaxonId { get; set; }

    public string DanishVernacularName { get; set; }    

    public Taxon? parent { get; set; }

    public List<Taxon> children { get; set; } = new();
}

