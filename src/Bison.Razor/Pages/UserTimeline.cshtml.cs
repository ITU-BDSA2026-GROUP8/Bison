using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IObservationService _service;
    public List<PostDTO> Observations { get; set; }

    public UserTimelineModel(IObservationService service)
    {
        _service = service;
    }
    

    public ActionResult OnGet(string author,[FromQuery] int page)
    {
        Observations = _service.GetObservationsFromAuthor(author, page);
        return Page();
    }
}
