using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Core.Models;
using Bison.Core.DTOs;
using Bison.Infrastructure.Services;
using System.ComponentModel.DataAnnotations;


namespace Bison.Razor.Pages;

public class AboutMeModel : PageModel
{
private readonly IObservationService _service;
    private readonly IObservationFilterService _filter;
    public List<PostDTO> Observations { get; set; }

    public AboutMeModel(IObservationService service, IObservationFilterService filter)
    {
        _service = service;
        _filter = filter;
    }
        public ActionResult OnGet()
    {
        
        return Page();
    }
    

    
}
