using Chirp.Web.Pages.BindingModels;
using Microsoft.AspNetCore.Mvc;

public class SubmitCheepViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(MessageModel messageModel, string targetPage)
    {
        if (!User.Identity!.IsAuthenticated) {
            return View("Unauthenticated");
        }
        ViewBag.TargetPage = targetPage;
        return View("Default", messageModel);
    }
}