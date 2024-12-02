using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class SideVideoViewComponent : ViewComponent
{
    private readonly List<string> _videoNames =
    [
        "~/videos/Kittens.mp4",
        "~/videos/SubwaySurfers_1.mp4",
        "~/videos/SubwaySurfers_2.mp4",
        "~/videos/TempleRun.mp4"
    ];
    
    public IViewComponentResult Invoke(bool isLeft)
    {
        ViewBag.IsLeft = isLeft;
        ViewBag.VideoList = GetVideos();

        return View("Default");
    }
    
    public List<string> GetVideos()
    {
        var random = new Random();
        var videos = _videoNames.OrderBy(x => random.Next()).ToList();
        return videos.Select(v =>Url.Content(v)).ToList();
    }
}