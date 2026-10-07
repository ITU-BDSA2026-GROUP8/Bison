using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleDB;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly ObservationService _observationService;
    private readonly CommentService _commentService;

    private readonly ProposalService _proposalService;
    public Observation Observation { get; private set; } = null!;
    public List<Comment> Comments { get; private set; } = [];

    public List<Proposal> Proposals{get;private set;}=[];

    public ObservationModel(ObservationService observationService, CommentService commentService,ProposalService proposalService)
    {
        _observationService = observationService;
        _commentService = commentService;
        _proposalService = proposalService;
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
