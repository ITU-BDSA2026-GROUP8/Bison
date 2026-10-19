using Bison.Razor.Models;


public static class ObservationFilter
{
    
    public static List<Observation> Filter(Taxon root, List<Observation> obs)
    {
        var input = Dafny.Sequence<Observation>.FromArray(obs.ToArray());
        var result = FilterWrapper.__default.FilterObservations(root, input);
        return result.CloneAsArray().ToList();    
        }
}
