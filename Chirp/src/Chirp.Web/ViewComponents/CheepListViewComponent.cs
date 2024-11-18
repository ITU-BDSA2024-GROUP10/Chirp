using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class CheepListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IEnumerable<CheepDTO> cheeps)
    {
        ViewBag.Cheeps = cheeps;
        return View("Default");
    }
}