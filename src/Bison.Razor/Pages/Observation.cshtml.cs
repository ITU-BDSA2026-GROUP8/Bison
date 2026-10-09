using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly IObservationService _observationService;
    private readonly CommentService _commentService;
    private readonly ProposalService _proposalService;

    public PostDTO Observation { get; private set; } = null!;
    public List<SimpleDB.Comment> Comments { get; private set; } = [];
    public List <SimpleDB.Taxon> Taxons;
    public List<SimpleDB.Proposal> Proposals{get;private set;}=[];

    public ObservationModel(IObservationService observationService, CommentService commentService)
    {
        _observationService = observationService;
        _commentService = commentService;
        _proposalService = proposalService;
        _taxonService=taxonService;
        Taxons=_taxonService.GetTaxons();
    }

    public IActionResult OnGet(int id)
    {
        var observation = _observationService.GetObservation(id);
        if (observation is null)
        {
            return NotFound();
        }

        Observation = observation;
        Comments = _commentService.GetCommentsForObservation(id);
        Proposals = _proposalService.GetProposalsForObservation(id);
        return Page();
    }
}
