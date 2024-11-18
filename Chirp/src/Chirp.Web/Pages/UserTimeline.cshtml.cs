﻿using System.Text;
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
            var returnUrl = Url.Content($"~/{author.ToLower()}?page=1");
            return LocalRedirect(returnUrl);
        }

        if (!author.All(c => char.IsLower(c) || !char.IsLetter(c)))
        {
            var returnUrl = Url.Content($"~/{author.ToLower()}?page={page}");
            return LocalRedirect(returnUrl);
        }
        
        PageNumber = page < 1 ? 1 : page;
        LoadCheeps(page);
        
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        var author = HttpContext.GetRouteValue("author")?.ToString();
        
        if (author == null)
        {
            throw new ArgumentNullException(nameof(author), "Author cannot be null, failed to get the route value");
        }

        Cheeps = Service.GetCheepsFromAuthorByPage(author, page, 32);
    }
}