using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages.Shared.Components.SubmitCheep;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService service) : TimeLinePageModel(service)
{
    public ActionResult OnGet([FromQuery] int page)
    {
        LoadCheeps(page);
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        Cheeps = Service.GetCheepsByPage(page, 32);
    }
}
