using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages.Shared.Components.Timelines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService service) : TimeLinePageModel(service)
{
    public int PageNumber;
    public ActionResult OnGet([FromQuery] int page)
    {
        LoadCheeps(page);
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        PageNumber = page < 1 ? 1 : page;
        Cheeps = Service.GetCheepsByPage(PageNumber, 32);
    }
}
