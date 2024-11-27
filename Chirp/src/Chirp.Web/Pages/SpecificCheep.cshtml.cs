using Chirp.Core;
using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class SpecificCheep(ICheepService cheepService) : PageModel
{
    public CheepDTO Cheep { get; set; } = null!;
    public int CommentCount { get; set; }
    public List<CommentDTO> Comments { get; set; } = [];
    public void OnGet(int cheepId)
    {
        Cheep = cheepService.GetCheepFromId(cheepId);
        CommentCount = cheepService.GetCommentAmountOnCheep(cheepId);
        Comments = cheepService.GetCommentsFromCheep(cheepId);
    }

    public string TimeSinceComment(long timeStamp)
    {
        var utcThen =  DateTimeOffset.FromUnixTimeSeconds(timeStamp);
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
    
    public IActionResult OnPostComment(string author, int cheepId, string comment)
    {
        var dt = DateTimeOffset.UtcNow;
        var commentDTO = new CommentDTO
        (
            author, 
            cheepId, 
            comment, 
            dt.ToUnixTimeSeconds()
        );

        if (!cheepService.AddCommentToCheep(commentDTO))
        {
            throw new ApplicationException("Failed to add comment");
        }
       
        return RedirectToPage("/SpecificCheep", new { cheepId = cheepId });
    }
}