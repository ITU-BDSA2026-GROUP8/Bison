using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; }

    public PublicModel(IObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        var all = _service.GetObservations();
        var paged = all.Skip((Page - 1)*32).Take(32).ToList();
        Observations = paged;
        return Page();
    }

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

}
