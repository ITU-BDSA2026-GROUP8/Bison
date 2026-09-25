using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; }

    public UserTimelineModel(IObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        var all = _service.GetObservationsFromAuthor(author);
        var aut = all.Skip((Page - 1)*32).Take(32).ToList(); 
        Observations = aut;
        return Page();
    }

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;
}
