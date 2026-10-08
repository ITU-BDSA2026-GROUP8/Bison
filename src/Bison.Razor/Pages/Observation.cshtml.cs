using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly ObservationService _observationService;
    private readonly CommentService _commentService;

    public Observation Observation { get; private set; } = null!;
    public List<SimpleDB.Comment> Comments { get; private set; } = [];

    public ObservationModel(ObservationService observationService, CommentService commentService)
    {
        _observationService = observationService;
        _commentService = commentService;
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
        return Page();
    }
}
