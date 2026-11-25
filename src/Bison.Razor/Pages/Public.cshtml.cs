using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Bison.Core.Models;
using Bison.Core.DTOs;
using Bison.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http;


namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;
    private readonly IObservationFilterService _filter;
    public List<PostDTO> Observations { get; set; }
    public int CurrentPage { get; private set; } = 1;

    [BindProperty]
    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    //[BindProperty]
    //public string? ImageUrl {get; set; }

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    [BindProperty]
    public string? ImageLicense { get; set; }

    
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

    public async Task<IActionResult> OnPostAsync()
    {
         if (User.Identity?.IsAuthenticated != true)
        return Challenge();   // sender brugeren til login

    if (ImageFile != null && string.IsNullOrWhiteSpace(ImageLicense))
    {
        ModelState.AddModelError(nameof(ImageLicense), "Image license is required!" );
    }

    if (ImageFile == null && !string.IsNullOrWhiteSpace(ImageLicense))
    {
        ModelState.AddModelError(nameof(ImageLicense), "Image license is not required, without a picture!" );
    }

    if (!ModelState.IsValid)
    {
        Observations = _service.GetObservations(1);
        return Page();
    }


    string? imageUrl = null;

    if (ImageFile != null)
    {
        Directory.CreateDirectory(
            Path.Combine("wwwroot", "uploads"));

        string fileName =
            Guid.NewGuid() +
            Path.GetExtension(ImageFile.FileName);

        string filePath =
            Path.Combine(
                "wwwroot",
                "uploads",
                fileName);

        using (var stream = System.IO.File.Create(filePath))
        {
            await ImageFile.CopyToAsync(stream);
        }

        imageUrl = "/uploads/" + fileName;
    }

    _service.StoreObservation(User.Identity.Name!, Message, Location, imageUrl, ImageLicense);
    return RedirectToPage();
    }
}
