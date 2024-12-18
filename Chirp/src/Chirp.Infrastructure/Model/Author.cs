using Chirp.Core.DTO;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Model;
/// <summary>
/// The Author class acts as the User for the Chirp! program
/// where an author is able to Create, Comment and/or Like Cheeps
/// or Follow other Authors
/// </summary>
public sealed class Author : IdentityUser
{
    public List<Cheep> Cheeps { get; set; } = [];
    public List<Author> Following { get; set; } = [];
    public List<Author> Followers { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public List<Like> Likes { get; set; } = [];
    public byte[]? ProfileImage { get; set; } = null;

    public Author()
    {
    }
    /// <summary>
    /// Constructor to instantiate a new Author given an AuthorDTO
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public static Author CreateAuthor(AuthorDTO author)
    {
        return CreateAuthor(author.Name, author.Email);
    }
    /// <summary>
    /// Constructor for creating a new Author object with the relevant information
    /// </summary>
    /// <param name="username"></param>
    /// <param name="email"></param>
    /// <returns>Returns the created Author with the given username and email</returns>
    /// <exception cref="ArgumentNullException">Thrown to specify that a username or email cannot be null</exception>
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