using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages.BindingModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class SpecificCheep(ICheepService cheepService) : PageModel
{
    public CheepDTO Cheep { get; set; } = null!;
    public int CommentCount { get; set; }
    public IEnumerable<CommentDTO> Comments { get; set; } = [];

    [BindProperty] public MessageModel MessageModel { get; set; } = new MessageModel();

    public void OnGet(int cheepId)
    {
        Cheep = cheepService.GetCheepFromId(cheepId);
        CommentCount = cheepService.GetCommentAmountOnCheep(cheepId);
        Comments = cheepService.GetCommentsFromCheep(cheepId);
    }
    /// <summary>
    /// Returns a formatted string according to the time since a comment was left
    /// (This method is currently inaccurate by 1 hour due to daylight savings time)
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public string TimeSinceComment(long timeStamp)
    {
        var utcThen = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
        var localTimeZone = TimeZoneInfo.Local;
        var then = TimeZoneInfo.ConvertTime(utcThen, localTimeZone);
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, localTimeZone);

        var timesince = (now - then) switch
        {
            { TotalMinutes: < 60 } ts => $"{ts.Minutes} minutes ago",
            { TotalHours: < 24 } ts => $"{ts.Hours} hours ago",
            { TotalDays: < 2 } => $"yesterday",
            { TotalDays: < 5 } => $"on {then.DayOfWeek}",
            var ts => $"{ts.Days} days ago",
        };
        return timesince;
    }
    /// <summary>
    /// Handles posting a Comment from the User to the current Cheep
    /// </summary>
    /// <param name="author"></param>
    /// <param name="cheepId"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public IActionResult OnPostComment(string author, int cheepId)
    {
        if (string.IsNullOrWhiteSpace(MessageModel.Message))
        {
            ModelState.AddModelError("Message", "Message cannot be empty");
            Reload(cheepId);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            Reload(cheepId);
            return Page();
        }


        var dt = DateTimeOffset.UtcNow;
        var commentDTO = new CommentDTO
        (
            author,
            cheepId,
            MessageModel.Message,
            dt.ToUnixTimeSeconds()
        );

        if (!cheepService.CreateComment(commentDTO))
        {
            throw new ApplicationException("Failed to add comment");
        }

        return RedirectToPage("/SpecificCheep", new { cheepId = cheepId });
    }
    /// <summary>
    /// Makes sure the correct user information is set to the model on reload
    /// </summary>
    /// <param name="cheepId"></param>
    private void Reload(int cheepId)
    {
        Cheep = cheepService.GetCheepFromId(cheepId);
        CommentCount = cheepService.GetCommentAmountOnCheep(cheepId);
        Comments = cheepService.GetCommentsFromCheep(cheepId);
    }
}