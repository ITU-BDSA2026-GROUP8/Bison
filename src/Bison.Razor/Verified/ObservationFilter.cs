using Bison.Razor.Models;


// Converts between C# lists and Dafny sequences, so the rest of the app
// never has to know about the generated Dafny code.
public static class ObservationFilter
{
    
    // Returns the observations whose taxon is root or a subtaxon of root.
    public static List<Observation> Filter(Taxon root, List<Observation> obs)
    {
        // List -> Dafny sequence
        var input = Dafny.Sequence<Observation>.FromArray(obs.ToArray());

        // Run the verified filter
        var result = FilterWrapper.__default.FilterObservations(root, input);

        // Dafny sequence -> List
        return result.CloneAsArray().ToList();    
        }
}
