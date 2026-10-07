using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ObservationService _service;
    private readonly CommentService _commentservice;
    public List<Observation> Observations { get; set; }

    [BindProperty]
    [Required]
    public string Message { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Location { get; set; } = string.Empty;

    public PublicModel(ObservationService service, CommentService commentservice)
    {
        _service = service;
        _commentservice = commentservice;
    }

    public ActionResult OnGet([FromQuery] int page)
    {       
        Observations = _service.GetObservations(page);
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
