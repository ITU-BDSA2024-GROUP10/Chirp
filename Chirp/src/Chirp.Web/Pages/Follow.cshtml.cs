using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[ValidateAntiForgeryToken]
public class Follow(IAuthorService authorService) : PageModel
{
    public ActionResult OnPost(string followName, bool shouldFollow, string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(followName))
        {
            return BadRequest("The followName parameter is required.");
        }

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = "/";
        }

        if (shouldFollow)
        {
            authorService.Follow(User.Identity!.Name!, followName);
        }
        else
        {
            authorService.Unfollow(User.Identity!.Name!, followName);
        }

        return LocalRedirect(returnUrl);
    }
}