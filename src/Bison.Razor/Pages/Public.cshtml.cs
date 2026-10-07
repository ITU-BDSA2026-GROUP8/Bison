using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SimpleDB;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ObservationService _service;
    public List<Observation> Observations { get; set; }

    [BindProperty]
    [Required]
    public string Message { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Location { get; set; } = string.Empty;

    public PublicModel(ObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int page)
    {
       
        var all = _service.GetObservations();
        var paged = all.Skip((page - 1) * 32).Take(32).ToList();
        Observations = paged;
        return Page();
    }

    public IActionResult OnPost()
    {
        Console.WriteLine("Public observation POST handler reached.");
        if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(Location))
        {
            ModelState.AddModelError(string.Empty, "Enter both an observation and a location.");
            Observations = _service.GetObservations().Take(32).ToList();
            return Page();
        }

        _service.StoreObservation(Environment.UserName, Message, Location);
        return RedirectToPage();
    }

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;
}
