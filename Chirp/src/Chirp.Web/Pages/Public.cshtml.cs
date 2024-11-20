using Chirp.Core;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService cheepService) : TimeLinePageModel(cheepService)
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
        LastPageNumber = CheepService.GetAmountOfCheepPages(PageSize);
        Cheeps = CheepService.GetCheepsByPage(page, PageSize);
    }
}
