using Chirp.Core;
using Chirp.Core.CustomException;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;
/// <summary>
/// The AuthorRepository class retrieves and stores author data in the ChirpDBContext
/// </summary>
/// <param name="context"></param>
public class AuthorRepository(ChirpDBContext context) : IAuthorRepository
{
    private readonly ChirpDBContext context = context;
    /// <summary>
    /// Gets all authors which the given Name
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var author = await GetAuthor(name);
        return new AuthorDTO(author.UserName!, author.Email!, author.ProfileImage!);
    }
    /// <summary>
    /// Gets all authors which the given Names
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public async Task<IEnumerable<AuthorDTO?>> GetAuthorsByNames(IEnumerable<string> names)
    {
        return await context.Authors
            .Where(a => names.Contains(a.UserName))
            .Select(a => new AuthorDTO(a.UserName!, a.Email!, a.ProfileImage))
            .ToListAsync();
    }
    /// <summary>
    /// Adds a new Author with the information in the AuthorDTO
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public async Task<bool> AddAuthor(AuthorDTO author)
    {
        try
        {
            context.Authors.Add(Author.CreateAuthor(author));
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    /// <summary>
    /// Returns the Users followed by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<IEnumerable<AuthorDTO>> GetAuthorFollows(string username)
    {
        if (!await UserExists(username)) throw new UserDoesNotExist();
        return GetFollowing(username).Result.Select(a => new AuthorDTO(a.UserName!, a.Email!, a.ProfileImage)).ToList();
    }
    /// <summary>
    /// Gets the Users that follow the given User
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<IEnumerable<AuthorDTO>> GetAuthorFollowers(string username)
    {
        if (!await UserExists(username)) throw new UserDoesNotExist();
        var followers = await context.Authors
            .Where(a => a.NormalizedUserName == username.ToUpper())
            .Select(a => a.Followers)
            .FirstOrDefaultAsync() ?? new List<Author>();

        return followers.Select(a => new AuthorDTO(a.UserName!, a.Email!, a.ProfileImage)).ToList();
    }
    /// <summary>
    /// Makes a User follow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToFollow"></param>
    /// <returns></returns>
    public async Task<bool> Follow(string currentUser, string userToFollow)
    {
        if (currentUser == userToFollow) throw new ArgumentException("You cannot follow yourself");
        if (!await UserExists(currentUser)) throw new UserDoesNotExist();
        if (!await UserExists(userToFollow)) throw new UserDoesNotExist("User to follow does not exist");
        if (await DoesFollow(currentUser, userToFollow)) return false;

        var authorToFollow = await GetAuthor(userToFollow);
        GetAuthor(currentUser).Result.Following.Add(authorToFollow);

        await context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Makes a User unfollow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToUnfollow"></param>
    /// <returns></returns>
    public async Task<bool> UnFollow(string currentUser, string userToUnfollow)
    {
        currentUser = currentUser.ToUpper();
        userToUnfollow = userToUnfollow.ToUpper();
        if (currentUser == userToUnfollow) throw new ArgumentException("You cannot unfollow yourself");
        if (!await UserExists(currentUser)) throw new UserDoesNotExist();
        if (!await UserExists(userToUnfollow)) throw new UserDoesNotExist("User to unfollow does not exist");
        if (!await DoesFollow(currentUser, userToUnfollow)) return false;

        var authorToUnfollow = await GetAuthor(userToUnfollow);
        context.Authors
            .Where(a => a.NormalizedUserName == currentUser)
            .Include(a => a.Following)
            .FirstOrDefault()!.Following.Remove(authorToUnfollow);

        await context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Gets the Author with the given UserName
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    /// <exception cref="UserDoesNotExist"></exception>
    private async Task<Author> GetAuthor(string username)
    {
        username = username.ToUpper();
        var author = await context.Authors.FirstOrDefaultAsync(a => a.NormalizedUserName == username);
        if (author == null) throw new UserDoesNotExist();
        return author;
    }
    /// <summary>
    /// Returns whether a User with the given UserName exits
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    private async Task<bool> UserExists(string username)
    {
        username = username.ToUpper();
        return await context.Authors.AnyAsync(a => a.NormalizedUserName == username);
    }
    /// <summary>
    /// Returns whether some User follows another User given the Names
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToFollow"></param>
    /// <returns></returns>
    private async Task<bool> DoesFollow(string currentUser, string userToFollow)
    {
        currentUser = currentUser.ToUpper();
        userToFollow = userToFollow.ToUpper();
        var list = await context.Authors
            .Where(a => a.NormalizedUserName == currentUser)
            .Select(a => a.Following).FirstOrDefaultAsync();

        return list != null && list.Any(a => a.NormalizedUserName == userToFollow);
    }
    /// <summary>
    /// Gets the Following List for the User with the given Name
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    private async Task<IEnumerable<Author>> GetFollowing(string currentUser)
    {
        currentUser = currentUser.ToUpper();
        return await context.Authors
            .Where(a => a.NormalizedUserName == currentUser)
            .Select(a => a.Following).FirstOrDefaultAsync() ?? new List<Author>();
    }
    /// <summary>
    /// Makes all Users which follow the given User, unfollow them
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<bool> MakeFollowersUnfollow(string user)
    {
        if (!await UserExists(user)) throw new UserDoesNotExist();
        (await context.Authors.Where(a => a.NormalizedUserName == user.ToUpper())
            .Include(a => a.Followers)
            .FirstOrDefaultAsync())!.Followers.Clear();
        await context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Gets all Comments made by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task<IEnumerable<CommentDTO>> GetComments(string username)
    {
        if (!await UserExists(username)) throw new UserDoesNotExist();
        var query = context.Authors
            .Where(a => a.NormalizedUserName == username.ToUpper())
            .Include(a => a.Comments)
            .ThenInclude(c => c.Author)
            .Select(a => a.Comments)
            .FirstOrDefaultAsync();
        var comments = query.Result;
        
        return comments!.Select(c => new CommentDTO(c.Author.UserName!, c.Id, c.Message, new DateTimeOffset(c.TimeStamp).ToUnixTimeSeconds())).ToList();
    }
}