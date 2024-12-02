using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class CheepListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IEnumerable<CheepDTO> cheeps, string targetPage)
    {
        ViewBag.TargetPage = targetPage;
        ViewBag.Cheeps = cheeps;
        
        return View("Default");
    }
}