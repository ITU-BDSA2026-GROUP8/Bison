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

module FilterModule {
    import opened Models

  predicate Matches(root: Taxon, o: Observation)
  reads * 
  {
    o.GetTaxon() == root || o.GetTaxon().IsSubTaxon(root)
  }

function FilterBy(root: Taxon, obs: seq<Observation>): seq<Observation>
reads *
    ensures forall o :: o in FilterBy(root, obs) ==> o in obs && Matches(root, o)
    ensures forall o :: o in obs && Matches(root, o) ==> o in FilterBy(root, obs)
  {
    if |obs| == 0 then []
    else if Matches(root, obs[0]) then [obs[0]] + FilterBy(root, obs[1..])
    else FilterBy(root, obs[1..])
  }
}

module FilterWrapper {
  import opened Models
  import FilterModule

  method FilterObservations(root: Taxon, obs: seq<Observation>) returns (result: seq<Observation>)
      ensures result == FilterModule.FilterBy(root, obs)
  {
      result := FilterModule.FilterBy(root, obs);
  }
}




