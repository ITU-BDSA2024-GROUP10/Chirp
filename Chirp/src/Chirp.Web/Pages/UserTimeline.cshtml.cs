using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages.Shared.Components.Timelines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        
        Author = author;
        LoadCheeps(page);
        
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        if (Author == null) {
            throw new ArgumentNullException(nameof(Author));
        }
        Cheeps = Service.GetCheepsFromAuthorByPage(Author, page, 32);
    }
}
