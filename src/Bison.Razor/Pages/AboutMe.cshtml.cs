using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Core.Models;
using Bison.Core.DTOs;
using Bison.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;



namespace Bison.Razor.Pages;

public class AboutMeModel : PageModel
{
private readonly IObservationService _service;
    private readonly IObservationFilterService _filter;
     private readonly UserManager<IdentityUser> userManager; 
     private readonly SignInManager<IdentityUser> signInManager;
   
     public int CurrentPage { get; private set; } = 1;
    public List<PostDTO> Observations { get; set; }
    public List<CommentDTO> Comments{get;set;}

    public AboutMeModel(IObservationService service, IObservationFilterService filter,UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager)
    {
        _service = service;
        _filter = filter;
        this.userManager=userManager;
        this.signInManager=signInManager;
    }
    
       public ActionResult OnGet(string author,[FromQuery] int page)
    {
        CurrentPage = Math.Max(1, page);
        Observations = _service.GetObservationsFromAuthor(author, CurrentPage);
        Comments =_service.GetCommentsFromAuthor(author);
        return Page();
    } 
            [HttpPost]
            public async Task<IActionResult> OnPost(string Id, string author){
            Console.WriteLine(userManager.ToString());
            var user = await userManager.FindByNameAsync(author);
    
        if(user!=null)
        {
            Console.WriteLine(author);
            _service.DeleteObservationsFromAuthor(author);
            _service.DeleteCommentsFromAuthor(author);
            _service.DeleteAuthor(author);

            var result = await userManager.DeleteAsync(user);
            await signInManager.SignOutAsync();
            if (result.Succeeded)
            {  

                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            }
        else
        {
        }

        


        return Page();
    }
}
