using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages.BindingModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Shared;

public abstract class TimeLinePageModel(ICheepService service) : PageModel
{
    public List<CheepDTO> Cheeps { get; set; } = [];
    
    protected readonly ICheepService Service = service;
    
    public string? Author { get; set; } 
    public int PageNumber = 1;

    [BindProperty] 
    public MessageModel MessageModel { get; set; } = new MessageModel();
    
    public ActionResult OnPost(string? author, [FromQuery] int page)
    {
        Author = author;
        
        if (string.IsNullOrWhiteSpace(MessageModel.Message)) {
            ModelState.AddModelError("Message", "Message cannot be empty");
        }
        
        if (!ModelState.IsValid) {
            LoadCheeps(page);
            return Page();
        }
        
        var dt = (DateTimeOffset)DateTime.UtcNow;
        if (User.Identity != null)
        {
            var cheep = new CheepDTO (
                User.Identity.Name ?? "no name",
                MessageModel.Message!,
                dt.ToUnixTimeSeconds()
            );

            Service.CreateCheep(cheep);
        }
        else
        {
            throw new ArgumentNullException();
        }

        return RedirectToPage(null); //redirects to the same page
    }

    protected abstract void LoadCheeps(int page);
}