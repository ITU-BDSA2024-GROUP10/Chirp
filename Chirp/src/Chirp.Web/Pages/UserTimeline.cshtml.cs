using Chirp.Core;
using Chirp.Web.Pages.Shared.Components.Timelines;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel(ICheepService service) : TimeLinePageModel(service)
{
    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        Author = author;
        PageNumber = page < 1 ? 1 : page;
        LoadCheeps(page);
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        if (Author == null)
        {
            throw new ArgumentNullException(nameof(Author));
        }

        Cheeps = Service.GetCheepsFromAuthorByPage(Author, page, 32);
    }
}