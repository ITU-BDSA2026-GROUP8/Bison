using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly IObservationService _observationService;

    public PostDTO Observation { get; private set; } = null!;
    public List<CommentDTO> Comments { get; private set; } = [];
    public TaxonDTO _Taxon;

    public ObservationModel(IObservationService observationService)
    {
        _observationService = observationService;
    }

    public IActionResult OnGet(int id)
    {
        var observation = _observationService.GetObservation(id);
        if (observation is null)
        {
            return NotFound();
        }
        
        Comments = _observationService.GetCommentsForObservation(id);
        _Taxon = _observationService.GetTaxonForObservation(id);
        return Page();
    }
}
