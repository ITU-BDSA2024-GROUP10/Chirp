using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class CheepListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IEnumerable<CheepDTO> cheeps, Dictionary<string, byte[]> imageMap, String targetPage)
    {
        ViewBag.TargetPage = targetPage;
        ViewBag.Cheeps = cheeps;
        ViewBag.ImageMap = imageMap;
        return View("Default");
    }
}