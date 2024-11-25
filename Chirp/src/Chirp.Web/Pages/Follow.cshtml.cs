using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[ValidateAntiForgeryToken]
public class Follow(IAuthorService authorService) : PageModel
{
    public ActionResult OnPost(string followName, bool shouldFollow, string? returnUrl)
    {
        // for debugging
        Console.WriteLine($"followName: {followName}, shouldFollow: {shouldFollow}, returnUrl: {returnUrl}");

        // Validate followName
        if (string.IsNullOrWhiteSpace(followName))
        {
            return BadRequest("The followName parameter is required.");
        }

        // validate returnUrl
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = "/";
        }

        if (shouldFollow)
        {
            Console.WriteLine($"{User.Identity!.Name} is now following {followName}.");
            authorService.Follow(User.Identity!.Name!, followName);
        }
        else
        {
            Console.WriteLine($"{User.Identity!.Name} has unfollowed {followName}.");
            authorService.Unfollow(User.Identity!.Name!, followName);
        }

        // redirect to the return URL
        return LocalRedirect(returnUrl);
    }
}