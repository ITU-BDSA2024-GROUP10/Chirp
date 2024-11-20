using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class AboutMe(IAuthorRepository authorRepository, ICheepRepository cheepRepository) : PageModel
{
    public AuthorDTO? Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = [];
    public async void OnGet()
    {
        Author = await authorRepository.GetAuthorByName(User.Identity!.Name!);
        Cheeps = (await cheepRepository.GetCheepsFromAuthor(Author!.Name)).ToList();
    }
}