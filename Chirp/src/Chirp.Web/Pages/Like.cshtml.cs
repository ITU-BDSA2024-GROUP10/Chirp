using Chirp.Core;
using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class LikeModel : PageModel
{
    private readonly ICheepService CheepService;

    public LikeModel(ICheepService cheepService)
    {
        CheepService = cheepService;
    }

    public async Task<IActionResult> OnPostToggleLikeAsync(int cheepId, bool isLiking)
    {
        try
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized();
            }

            var author = User.Identity.Name;
            var likeDto = new LikeDTO(author, cheepId);

            if (isLiking)
            {
                await CheepService.LikeCheep(likeDto);
            }
            else
            {
                await CheepService.UnlikeCheep(likeDto);
            }

            var likeCount = await CheepService.GetLikeCount(cheepId);

            return new JsonResult(new { likeCount });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error toggling like: {ex}");
            return StatusCode(500, "An error occurred while toggling like.");
        }
    }
}