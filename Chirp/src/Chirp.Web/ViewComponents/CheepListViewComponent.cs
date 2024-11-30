using Chirp.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class CheepListViewComponent : ViewComponent
{
    private readonly List<string> _videoNames =
    [
        "~/videos/Kittens.mp4",
        "~/videos/SubwaySurfers_1.mp4",
        "~/videos/SubwaySurfers_2.mp4",
        "~/videos/TempleRun.mp4"
    ];
    
    public IViewComponentResult Invoke(IEnumerable<CheepDTO> cheeps, string targetPage)
    {
        ViewBag.TargetPage = targetPage;
        ViewBag.Cheeps = cheeps;
        ViewBag.videoListLeft = GetVideos().Select(v =>Url.Content(v)).ToList();
        ViewBag.videoListRight = GetVideos().Select(v =>Url.Content(v)).ToList();
        
        return View("Default");
    }

    public List<string> GetVideos()
    {
        var random = new Random();
        return _videoNames.OrderBy(x => random.Next()).ToList();
    }
}