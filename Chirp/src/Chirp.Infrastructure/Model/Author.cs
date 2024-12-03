using Chirp.Core.DTO;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Model;

public sealed class Author : IdentityUser
{
    public List<Cheep> Cheeps { get; set; } = [];
    public List<Author> Following { get; set; } = [];
    public List<Author> Followers { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public byte[]? ProfileImage { get; set; } = null;

    public Author()
    {
    }

    public static Author CreateAuthor(AuthorDTO author)
    {
        return CreateAuthor(author.Name, author.Email);
    }

    public static Author CreateAuthor(string username, string email)
    {
        _ = username ?? throw new ArgumentNullException(nameof(username));
        _ = email ?? throw new ArgumentNullException(nameof(email));

        return new Author()
        {
            Email = email,
            UserName = username,
            NormalizedUserName = username.ToUpper(),
            NormalizedEmail = email.ToUpper()
        };
    }
}