using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleDB.DTO;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepDTO> Cheeps { get; set; }

    public ActionResult OnGet([FromQuery] int page)
    {
        Cheeps = _service.GetCheepsByPage(page, 32);
        return Page();
    }
}
