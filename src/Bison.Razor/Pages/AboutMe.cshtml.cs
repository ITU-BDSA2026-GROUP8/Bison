using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Core.Models;
using Bison.Core.DTOs;
using Bison.Infrastructure.Services;


namespace Bison.Razor.Pages;

public class AboutMeModel : PageModel
{
private readonly IObservationService _service;
    private readonly IObservationFilterService _filter;
    public int CurrentPage { get; private set; } = 1;
    public List<PostDTO> Observations { get; set; }
    public List<CommentDTO> Comments{get;set;}

    public AboutMeModel(IObservationService service, IObservationFilterService filter)
    {
        _service = service;
        _filter = filter;
    }
    
       public ActionResult OnGet(string author,[FromQuery] int page)
    {
        CurrentPage = Math.Max(1, page);
        Observations = _service.GetObservationsFromAuthor(author, CurrentPage);
        Comments =_service.GetCommentsFromAuthor(author);
        return Page();
    } 
}
