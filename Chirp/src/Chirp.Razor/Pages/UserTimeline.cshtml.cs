using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleDB.Model;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }
    [BindProperty] 
    public int PageNumber { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, string pageNumber)
    {
        if (!string.IsNullOrEmpty(pageNumber))
        {
            PageNumber = int.Parse(pageNumber);
            Cheeps = _service.GetCheepsFromAuthorByPage(author, PageNumber, 12);
            return Page();
        }
        Cheeps = _service.GetCheepsFromAuthor(author);
        return Page();
    }
}
