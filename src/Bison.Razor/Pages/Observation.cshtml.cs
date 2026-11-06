using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Razor.Models;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly IObservationService _observationService;
    public String Message {get;set;}=String.Empty;
    public String Name {get;set;}=String.Empty;
    public int ID;
    public PostDTO Observation { get; private set; } = null!;
    public List<CommentDTO> Comments { get; private set; } = [];
    public TaxonDTO? _Taxon;

    public ObservationModel(IObservationService observationService)
    {
        _observationService = observationService;
    }
    public IActionResult OnPost()
    {
        Console.WriteLine("metoden kommer igennem i guess");
        if(string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Message)){
            ModelState.AddModelError(string.Empty, "Enter both an observation and a location.");

            return Page();
        }
        Console.WriteLine("jeg er i det mindste her");
        if(!string.IsNullOrWhiteSpace(Name)){
            Console.WriteLine(ID+" "+Name + " her er jeg");
            _observationService.StoreProposal(ID,Name);
        }
        return RedirectToPage();
    }

    public IActionResult OnGet(int id)
    {
        ID=id;
        var observation = _observationService.GetObservation(id);
        if (observation is null)
        {
            return NotFound();
        }
        Observation = observation;
        Comments = _observationService.GetCommentsForObservation(id);
        _Taxon = _observationService.GetTaxonForObservation(id);
        return Page();
    }
}
