using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;
    private readonly IObservationFilterService _filter;
    public List<PostDTO> Observations { get; set; }
    public int CurrentPage { get; private set; } = 1;

    [BindProperty]
    [Required]
    public string Message { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Location { get; set; } = string.Empty;

    public PublicModel(IObservationService service, IObservationFilterService filter)
    {
        _service = service;
        _filter = filter;
    }

    public ActionResult OnGet([FromQuery] int page, [FromQuery] string? taxon)
    {
        CurrentPage = Math.Max(1, page);
        Observations = string.IsNullOrEmpty(taxon)
            ? _service.GetObservations(CurrentPage)
            : _filter.GetObservationsByTaxon(taxon);
        return Page();
    }

    public IActionResult OnPost()
    {
        Console.WriteLine("Public observation POST handler reached.");
        if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(Location))
        {
            ModelState.AddModelError(string.Empty, "Enter both an observation and a location.");
            Observations = _service.GetObservations(1);
            return Page();
        }

        _service.StoreObservation(Environment.UserName, Message, Location);
        return RedirectToPage();
    }
}
