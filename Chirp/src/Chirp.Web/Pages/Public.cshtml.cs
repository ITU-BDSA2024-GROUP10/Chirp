using Chirp.Core;
using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService service) : PageModel
{
    public List<CheepDTO> Cheeps { get; set; } = [];

    public ActionResult OnGet([FromQuery] int page)
    {
        if (page < 1)
        {
            var returnUrl = Url.Content("~/?page=1");
            return LocalRedirect(returnUrl);
        }
        
        Cheeps = service.GetCheepsByPage(page, 32);
        
        return Page();
    }
}
