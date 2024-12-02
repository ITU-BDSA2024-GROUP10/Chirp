using Chirp.Core;
using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class LikeModel : PageModel
{
    private readonly ICheepService _cheepService;

    public LikeModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
    }

    public async Task<IActionResult> OnPostAsync(int cheepId, bool isLiking, string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = "/";
        }

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var author = User.Identity.Name!;
        var likeDto = new LikeDTO(author, cheepId);

        if (isLiking)
        {
            await _cheepService.LikeCheep(likeDto);
        }
        else
        {
            await _cheepService.UnlikeCheep(likeDto);
        }

        return LocalRedirect(returnUrl);
    }
}