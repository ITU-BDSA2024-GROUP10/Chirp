using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication;

namespace Chirp.Web.Pages;

public class AboutMe(IAuthorService authorService, ICheepService cheepService, SignInManager<Author> signInManager, UserManager<Author> userManager) : PageModel
{
    public Author? Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = [];
    public async void OnGet()
    {
        Author = await userManager.FindByNameAsync(User.Identity!.Name!);
        Cheeps = (cheepService.GetCheepsFromAuthor(Author!.UserName!)).ToList();
    }

    public async Task<ActionResult> OnPost()
    {
        await SignOutAndDeleteUser();
        return Redirect("/");
    }

    public async Task SignOutAndDeleteUser()
    {
        var authorName = User.Identity!.Name!;
        await signInManager.SignOutAsync();
        Author = await userManager.FindByNameAsync(authorName);
        if (Author != null)
        {
            authorService.MakeFollowersUnfollow(Author.UserName!);
            await userManager.DeleteAsync(Author);
        } else throw new Exception("Author could not be found");
    }
}
