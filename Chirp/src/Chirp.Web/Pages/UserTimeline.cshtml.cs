using Chirp.Core;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel(ICheepService service) : TimeLinePageModel(service)
{
    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        if (page < 1)
        {
            var returnUrl = Url.Content($"~/{author}?page=1");
            return LocalRedirect(returnUrl);
        }

        PageNumber = page < 1 ? 1 : page;
        LoadCheeps(page);

        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        var author = HttpContext.GetRouteValue("author");

        if (author == null)
        {
            throw new ArgumentNullException(nameof(author), "Couldn't get the author from the route");
        }

        Cheeps = Service.GetCheepsFromAuthorByPage(author.ToString()!, page, 32);
    }
}