using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication;
using NUnit.Framework;

namespace Chirp.Web.Pages;

public class AboutMe(IAuthorService authorService, ICheepService cheepService, SignInManager<Author> signInManager, UserManager<Author> userManager) : PageModel
{
    public Author? Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = [];
    public int AmountYouFollow { get; set; } = 0;
    public int AmountOfFollowers { get; set; } = 0;
    [BindProperty]
    [Required(ErrorMessage = "Please upload an image.")]
    [DataType(DataType.Upload)]
    public IFormFile? Avatar { get; set; }
    public async void OnGet()
    {
        Author = await userManager.FindByNameAsync(User.Identity!.Name!);
        if (Author?.UserName == null)
        {
            Redirect("/NotFound");
            return;
        }
        
        SetUserInfo(Author.UserName);
    }

    public async Task<IActionResult> OnPostImage()
    {
        Author = await userManager.FindByNameAsync(User.Identity!.Name!);
        if (Author == null)
        {
            return Redirect("/NotFound");
        }
        
        if (Avatar == null || Avatar.Length == 0)
        {
            ModelState.AddModelError("Avatar", "Please upload a valid image.");
            return Page();
        }
        
        if (!Avatar.FileName.Contains(".png") || !Avatar.FileName.Contains(".jpg"))
        {
            ModelState.AddModelError("Avatar", "Please upload a valid image. Only .png and .jpg are allowed.");
            return Page();
        }
        
        using var memoryStream = new MemoryStream();
        await Avatar.CopyToAsync(memoryStream);
        Author.ProfileImage = memoryStream.ToArray();
        await userManager.UpdateAsync(Author);
        
        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnPostConfirmDelete()
    {
        try
        {
            await SignOutAndDeleteUser();
        }
        catch
        {
            return Redirect("/NotFound");
        }
        
        return Redirect("/");
    }

    public async Task SignOutAndDeleteUser()
    {
        var authorName = User.Identity!.Name!;
        
        Author = await userManager.FindByNameAsync(authorName);
        if (Author?.UserName != null)
        {
            await signInManager.SignOutAsync();
            authorService.MakeFollowersUnfollow(Author.UserName);
            await userManager.DeleteAsync(Author);
        } else throw new Exception("Author data is not valid");
    }

    private void SetUserInfo(string authorUsername)
    {
        Cheeps = cheepService.GetCheepsFromAuthor(authorUsername);
        AmountYouFollow = authorService.GetFollows(authorUsername).Count;
        AmountOfFollowers = authorService.GetFollowers(authorUsername).Count;
    }
}
