using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ObservationService _service;
    public List<Observation> Observations { get; set; }

    public UserTimelineModel(ObservationService service)
    {
        _service = service;
    }
    

    public ActionResult OnGet(string author,[FromQuery] int page)
    {
        Observations = _service.GetObservationsFromAuthor(author, page);
        return Page();
    }
}
