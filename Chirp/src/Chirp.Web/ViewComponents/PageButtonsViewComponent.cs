using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class PageButtonsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int pageNumber, string targetPage, bool isLastPage)
    {
        ViewBag.TargetPage = targetPage;
        ViewBag.PageNumber = pageNumber;
        ViewBag.IsLastPage = isLastPage;
        return View("Default");
    }
}