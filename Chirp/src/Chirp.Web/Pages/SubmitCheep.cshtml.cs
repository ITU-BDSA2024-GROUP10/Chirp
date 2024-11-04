using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages.BindingModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Chirp.Web.Pages;

public class SubmitCheepModel(ICheepService service) : PageModel {
    
    public ActionResult OnPost(MessageModel messageModel)
    {
        if (string.IsNullOrWhiteSpace(messageModel.Message)) {
            ModelState.AddModelError("Message", "Message cannot be empty");
        }
        
        if (!ModelState.IsValid) {
            return Page();
        }
        
        var dt = (DateTimeOffset)DateTime.UtcNow;
        var cheep = new CheepDTO (
            User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value ?? "no name",
            messageModel.Message!,
            dt.ToUnixTimeSeconds()
        );

        service.CreateCheep(cheep);
        return RedirectToPage("/Public");

    }

}