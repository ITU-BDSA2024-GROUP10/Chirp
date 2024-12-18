using Chirp.Core;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class PublicModel(ICheepService cheepService, IAuthorService authorService) : TimeLinePageModel(cheepService, authorService)
{
    /// <summary>
    /// Displays the correct cheeps according to the page specified in the URL
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public ActionResult OnGet([FromQuery] int page)
    {
        if (page < 1)
        {
            var returnUrl = Url.Content("~/?page=1");
            return LocalRedirect(returnUrl);
        }
        PageNumber = page;
        LoadCheeps(PageNumber);
        LoadProfileImages(Cheeps);
        return Page();
    }
    /// <summary>
    /// Loads Cheeps to the Model via the CheepService according to the current page 
    /// </summary>
    /// <param name="page"></param>
    protected override void LoadCheeps(int page)
    {
        LastPageNumber = CheepService.GetAmountOfCheepPages(PageSize);
        Cheeps = CheepService.GetCheepsByPage(page, PageSize);
    }
}
