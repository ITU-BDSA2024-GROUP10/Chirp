using System.Text;
using Chirp.Core;
using Chirp.Core.CustomException;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel(ICheepService cheepService, IAuthorService authorService) : TimeLinePageModel(cheepService, authorService)
{
    protected new readonly IAuthorService AuthorService = authorService;
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
        
        PageNumber = page;
        
        try
        {
            LoadCheeps(page);
            LoadProfileImages(Cheeps);
        }
        catch (AggregateException ae)
        {
            bool shouldRedirect = false;
            // https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/exception-handling-task-parallel-library
            foreach (var ex in ae.InnerExceptions)
            {
                if (ex is UserDoesNotExist)
                {
                    shouldRedirect = true;
                }
                else
                {
                    throw ex;
                }
            }
            if (shouldRedirect) return LocalRedirect("/notfound");
        }
        
        return Page();
    }

    protected override void LoadCheeps(int page)
    {
        var author = HttpContext.GetRouteValue("author")?.ToString();
        
        if (author == null)
        {
            throw new ArgumentNullException(nameof(author), "Author cannot be null, failed to get the route value");
        }

        var authors = AuthorService.GetFollows(author)
            .Select(a => a.Name)
            .Append(author);
        LastPageNumber = CheepService.GetAmountOfCheepPagesFromAuthors(authors, PageSize);
        Cheeps = CheepService.GetCheepsFromAuthorsByPage(authors, page, PageSize);
    }
    
    public string NormalizeForDisplay(string author)
    {
        var parts = author.Split(' ');
        var result = new StringBuilder();
        foreach (var part in parts)
        {
            result.Append(char.ToUpper(part[0]));
            result.Append(part.Substring(1).ToLower());
            result.Append(' ');
        }
        return result.ToString().Trim();
    }
}