using Chirp.Core;
using Chirp.Web.Pages.Shared.Components.Timelines;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService service) : TimeLinePageModel(service)
{
    public ActionResult OnGet([FromQuery] int page)
    {
        if (page < 1)
        {
            var returnUrl = Url.Content("~/?page=1");
            return LocalRedirect(returnUrl);
        }
        PageNumber = page;
        LoadCheeps(PageNumber);
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        Cheeps = Service.GetCheepsByPage(page, 32);
    }
}
