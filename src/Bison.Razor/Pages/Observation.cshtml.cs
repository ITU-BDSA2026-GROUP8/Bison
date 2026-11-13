using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Core.Models;
using Bison.Core.DTOs;
using Bison.Core.Services;
using System.ComponentModel.DataAnnotations;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
    private readonly IObservationService _observationService;
    
    [BindProperty]
    [Required]
        public string Message { get; set; } = string.Empty;
        [BindProperty]
        [Required]
    public string Comment { get; set; } = string.Empty;

    public PostDTO Observation { get; private set; } = null!;
    public List<CommentDTO> Comments { get; private set; } = [];
    public TaxonDTO? _Taxon;

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
        Observation = observation;
        Comments = _observationService.GetCommentsForObservation(id);
        _Taxon = _observationService.GetTaxonForObservation(id);
        return Page();
    }
    public IActionResult OnPost(int id)
    {
        
        if(string.IsNullOrWhiteSpace(Message)&&string.IsNullOrWhiteSpace(Comment)){
            Console.WriteLine("jeg er her");
            ModelState.AddModelError(string.Empty, "Enter either a taxon or a comment.");

            return RedirectToPage();
        }
        Console.WriteLine("jeg er i det mindste her");
        if(!string.IsNullOrWhiteSpace(Message)){
            Console.WriteLine(id+" "+Message + " her er jeg");
            _observationService.StoreProposal(id,Message);
        }
        if (!string.IsNullOrWhiteSpace(Comment))
        {
            _observationService.StoreComment(Environment.UserName,Comment,id);
        }
        return RedirectToPage();
    }

    
}
