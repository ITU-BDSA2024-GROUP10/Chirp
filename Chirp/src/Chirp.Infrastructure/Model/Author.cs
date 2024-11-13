using Chirp.Core.DTO;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Model;

public sealed class Author : IdentityUser
{
    public List<Cheep> Cheeps { get; set; } = [];

    public Author()
    {
    }

    public static Author CreateAuthor(AuthorDTO author)
    {
        return CreateAuthor(author.Name, author.Email);
    }

    public static Author CreateAuthor(string name, string email)
    {
        _ = name ?? throw new ArgumentNullException(nameof(name));
        _ = email ?? throw new ArgumentNullException(nameof(email));

        return new Author()
        {
            Name = name,
            Email = email,
            UserName = email
        };
    }
}