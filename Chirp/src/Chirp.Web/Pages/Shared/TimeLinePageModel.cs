using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages.BindingModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Shared;

public abstract class TimeLinePageModel(ICheepService cheepService, IAuthorService authorService) : PageModel
{
    public List<CheepDTO> Cheeps { get; set; } = [];
    public Dictionary<string, byte[]> ImageMap = new();
    
    protected readonly ICheepService CheepService = cheepService;
    protected readonly IAuthorService AuthorService = authorService;
    
    public int PageNumber = 1;
    public int LastPageNumber = 1;
    protected const int PageSize = 32;

    [BindProperty] public MessageModel MessageModel { get; set; } = new MessageModel();

    public ActionResult OnPost(string? author, [FromQuery] int page)
    {
        if (string.IsNullOrWhiteSpace(MessageModel.Message))
        {
            ModelState.AddModelError("Message", "Message cannot be empty");
        }

        if (!ModelState.IsValid)
        {
            LoadCheeps(page);
            return Page();
        }

        var dt = (DateTimeOffset)DateTime.UtcNow;
        if (User.Identity != null)
        {
            var cheep = new CheepDTO(
                null,
                User.Identity.Name ?? "no name",
                MessageModel.Message!,
                dt.ToUnixTimeSeconds()
            );

            CheepService.CreateCheep(cheep);
        }
        else
        {
            throw new ArgumentNullException();
        }

        return RedirectToPage(null); //redirects to the same page
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

        if (!CheepService.AddCommentToCheep(commentDTO))
        {
            throw new ApplicationException("Failed to add comment");
        }

        return RedirectToPage(null);
    }

    protected abstract void LoadCheeps(int page);

    protected void LoadProfileImages(IEnumerable<CheepDTO> cheeps)
    {
        var authors = AuthorService.GetAuthorsByNames(cheeps.Select(c => c.Author));
        foreach (var author in authors)
        {
            if (author!.ProfileImage == null) continue;
            ImageMap[author.Name] = author.ProfileImage;
        }
    }
}