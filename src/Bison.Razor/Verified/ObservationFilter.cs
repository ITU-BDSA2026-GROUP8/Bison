using Bison.Razor.Models;


public static class ObservationFilter
{
    
    public static List<Observation> Filter(Taxon root, List<Observation> obs)
    {
        var input = Dafny.Sequence<Observation>.FromArray(obs.ToArray());
        var result = FilterModule.__default.FilterBy(root, input);
        return result.CloneAsArray().ToList();    
        }
}
