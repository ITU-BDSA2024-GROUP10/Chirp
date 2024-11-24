using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Chirp.Web.Pages;

public class AboutMe(IAuthorService authorService, ICheepService cheepService, SignInManager<Author> signInManager, UserManager<Author?> userManager) : PageModel
{
    public Author? Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = [];
    public async void OnGet()
    {
        Author = await userManager.FindByNameAsync(User.Identity!.Name!);
        Cheeps = (cheepService.GetCheepsFromAuthor(Author!.UserName!)).ToList();
    }

    public ActionResult OnPost()
    {
        Console.WriteLine($"Button clicked with value");
        return LocalRedirect("/");
    }
}