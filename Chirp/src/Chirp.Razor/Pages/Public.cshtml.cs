using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleDB.Model;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }
    
    [BindProperty] 
    public int PageNumber { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string pageNumber)
    {
        Console.WriteLine(pageNumber);
        if (!string.IsNullOrEmpty(pageNumber))
        {
            PageNumber = int.Parse(pageNumber);
            Cheeps = _service.GetCheepsByPage(PageNumber, 12);
            return Page();
        }
        Cheeps = _service.GetAllCheeps();
        return Page();
    }
}
