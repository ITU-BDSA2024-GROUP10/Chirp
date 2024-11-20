using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class PageButtonsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int currentPageNumber, int lastPageNumber, string targetPageUrl)
    {
        ViewBag.TargetPageUrl = targetPageUrl;
        ViewBag.CurrentPageNumber = currentPageNumber;
        ViewBag.LastPageNumber = lastPageNumber;
        return View("Default");
    }
}