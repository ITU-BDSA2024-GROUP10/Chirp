﻿using Chirp.Core;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel(ICheepService cheepService, IAuthorService authorService) : TimeLinePageModel(cheepService)
{
    protected readonly IAuthorService AuthorService = authorService;
    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        if (page < 1)
        {
            var returnUrl = Url.Content($"~/{author}?page=1");
            return LocalRedirect(returnUrl);
        }
        
        PageNumber = page;
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

        var authors = AuthorService.GetFollows(author).Select(a => a.Name).Append(author);
        Cheeps = CheepService.GetCheepsFromAuthorsByPage(authors, page, 32);
    }
}