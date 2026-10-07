using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleDB;

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
        var all = _service.GetObservationsFromAuthor(author);
        var aut = all.Skip((page - 1)*32).Take(32).ToList(); 
        Observations = aut;
        return Page();
    }

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;
}
