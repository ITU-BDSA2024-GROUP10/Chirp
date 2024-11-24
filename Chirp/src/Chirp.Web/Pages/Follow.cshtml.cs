using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class Follow(IAuthorService authorService) : PageModel
{
    public ActionResult OnPost(String followName, bool shouldFollow, string returnUrl)
    {
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