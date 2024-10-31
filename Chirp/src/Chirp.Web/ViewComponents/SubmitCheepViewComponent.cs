using Chirp.Web.Pages.BindingModels;
using Microsoft.AspNetCore.Mvc;

public class SubmitCheepViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(MessageModel messageModel)
    {
        if (!User.Identity!.IsAuthenticated) {
            return View("Unauthenticated");
        }
        return View("Default", messageModel);
    }
}