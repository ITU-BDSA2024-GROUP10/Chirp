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
    /// <summary>
    /// Likes or Unlikes a Cheep according to the state of the Cheep
    /// </summary>
    /// <param name="cheepId"></param>
    /// <param name="isLiking"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostToggleLikeAsync(int cheepId, bool isLiking)
    {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }

            var author = User.Identity!.Name;
            var likeDto = new LikeDTO(author!, cheepId);

            if (isLiking)
            {
                CheepService.LikeCheep(likeDto);
            }
            else
            {
                CheepService.UnlikeCheep(likeDto);
            }

            var likeCount = await CheepService.GetLikeCount(cheepId);

            return new JsonResult(new { likeCount });
    }
}