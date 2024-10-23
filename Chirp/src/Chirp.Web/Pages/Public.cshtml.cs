using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleDB.DTO;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService service) : PageModel
{
    public List<CheepDTO> Cheeps { get; set; } = [];

    public ActionResult OnGet([FromQuery] int page)
    {
        Cheeps = service.GetCheepsByPage(page, 32);
        return Page();
    }
}
