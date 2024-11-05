using Chirp.Core;
using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel(ICheepService service) : PageModel
{
    private readonly ICheepService _service = service;
    public required List<CheepDTO> Cheeps { get; set; }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        if (page < 1)
        {
            var returnUrl = Url.Content($"~/{author}?page=1");
            return LocalRedirect(returnUrl);
        }
        
        Cheeps = _service.GetCheepsFromAuthorByPage(author, page, 32);
        return Page();
    }
}
