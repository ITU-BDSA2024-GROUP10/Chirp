using Chirp.Core;
using Chirp.Web.Pages.Shared.Components.Timelines;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService service) : TimeLinePageModel(service)
{
    public ActionResult OnGet([FromQuery] int page)
    {
        PageNumber = page < 1 ? 1 : page;
        LoadCheeps(PageNumber);
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        Cheeps = Service.GetCheepsByPage(page, 32);
    }
}