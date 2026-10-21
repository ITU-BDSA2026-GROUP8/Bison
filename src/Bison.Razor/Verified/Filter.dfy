// The C# classes Taxon and Observation already exist in Bison.Razor.Models.
// Here we only declare their signatures so Dafny knows which methods it may call.
module {:extern "Bison.Razor.Models"} Models {
class {:extern "Taxon"} Taxon {
    function {:axiom} {:extern "IsSubTaxon"} IsSubTaxon(ancestor: Taxon): bool
    reads *
}

class {:extern "Observation"} Observation {
    function {:axiom} {:extern "GetTaxon"} GetTaxon(): Taxon
    reads *
}
}

// The verified core: the filter itself and its proof.
module FilterModule {
    import opened Models

// An observation matches if its taxon is the root or a subtaxon of the root.
  predicate Matches(root: Taxon, o: Observation)
  reads * 
  {
    o.GetTaxon() == root || o.GetTaxon().IsSubTaxon(root)
  }

// Keeps the observations that match root.
function FilterBy(root: Taxon, obs: seq<Observation>): seq<Observation>
reads *
    // Nothing wrong ends up in the result
    ensures forall o :: o in FilterBy(root, obs) ==> o in obs && Matches(root, o)
    // Nothing that matches is lost
    ensures forall o :: o in obs && Matches(root, o) ==> o in FilterBy(root, obs)
  {
    if |obs| == 0 then []
    else if Matches(root, obs[0]) then [obs[0]] + FilterBy(root, obs[1..])
    else FilterBy(root, obs[1..])
  }
}

// A wrapper around the verified core, so that C# can call it.
module FilterWrapper {
  import opened Models
  import FilterModule

  method FilterObservations(root: Taxon, obs: seq<Observation>) returns (result: seq<Observation>)
      ensures result == FilterModule.FilterBy(root, obs)
  {
      result := FilterModule.FilterBy(root, obs);
  }
}




