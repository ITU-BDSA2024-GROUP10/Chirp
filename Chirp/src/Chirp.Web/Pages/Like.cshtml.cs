using Chirp.Core;
using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

[ValidateAntiForgeryToken]
public class LikeModel : PageModel
{
    private readonly ICheepService _cheepService;

    public LikeModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
    }

    public async Task<IActionResult> OnPostLikeAsync(int cheepId, string? returnUrl)
    {
        try
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/";
            }

            var userName = User.Identity.Name!;
            var likeDto = new LikeDTO(userName, cheepId); 

            var success = await _cheepService.LikeCheep(likeDto);
            if (!success)
            {
                return BadRequest("Unable to like the cheep.");
            }

            var likeCount = await _cheepService.GetLikeCount(cheepId);
            return new JsonResult(new { likeCount });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error liking cheep: {ex.Message}");
            return StatusCode(500, "An error occurred while liking the cheep.");
        }
    }

    public async Task<IActionResult> OnPostUnlikeAsync(int cheepId, string? returnUrl)
    {
        try
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/";
            }

            var userName = User.Identity.Name!;
            var likeDto = new LikeDTO(userName, cheepId); 
            var success = await _cheepService.UnlikeCheep(likeDto);
            if (!success)
            {
                return BadRequest("Unable to unlike the cheep.");
            }

            var likeCount = await _cheepService.GetLikeCount(cheepId);
            return new JsonResult(new { likeCount });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error unliking cheep: {ex.Message}");
            return StatusCode(500, "An error occurred while unliking the cheep.");
        }
    }
}