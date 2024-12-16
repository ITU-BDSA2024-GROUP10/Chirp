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
   
    public async Task<IEnumerable<AuthorDTO?>> GetAuthorsByNames(IEnumerable<string> usernames)
    {
        return await context.Authors
            .Where(a => usernames.Contains(a.UserName))
            .Select(a => new AuthorDTO(a.UserName!, a.Email!, a.ProfileImage))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<AuthorDTO>> GetAuthorFollows(string username)
    {
        if (!await UserExists(username)) throw new UserDoesNotExist();
        return GetFollowing(username).Result.Select(a => new AuthorDTO(a.UserName!, a.Email!, a.ProfileImage)).ToList();
    }
    public async Task<IEnumerable<AuthorDTO>> GetAuthorFollowers(string username)
    {
        if (!await UserExists(username)) throw new UserDoesNotExist();
        var followers = await context.Authors
            .Where(a => a.NormalizedUserName == username.ToUpper())
            .Select(a => a.Followers)
            .FirstOrDefaultAsync() ?? new List<Author>();

        return followers.Select(a => new AuthorDTO(a.UserName!, a.Email!, a.ProfileImage)).ToList();
    }
    public async Task<bool> Follow(string userWhoFollow, string userToFollow)
    {
        if (userWhoFollow == userToFollow) throw new ArgumentException("You cannot follow yourself");
        if (!await UserExists(userWhoFollow)) throw new UserDoesNotExist();
        if (!await UserExists(userToFollow)) throw new UserDoesNotExist("User to follow does not exist");
        if (await DoesFollow(userWhoFollow, userToFollow)) return false;

        var authorToFollow = await GetAuthor(userToFollow);
        GetAuthor(userWhoFollow).Result.Following.Add(authorToFollow);

        await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UnFollow(string userWhoFollows, string userToUnfollow)
    {
        userWhoFollows = userWhoFollows.ToUpper();
        userToUnfollow = userToUnfollow.ToUpper();
        if (userWhoFollows == userToUnfollow) throw new ArgumentException("You cannot unfollow yourself");
        if (!await UserExists(userWhoFollows)) throw new UserDoesNotExist();
        if (!await UserExists(userToUnfollow)) throw new UserDoesNotExist("User to unfollow does not exist");
        if (!await DoesFollow(userWhoFollows, userToUnfollow)) return false;

        var authorToUnfollow = await GetAuthor(userToUnfollow);
        context.Authors
            .Where(a => a.NormalizedUserName == userWhoFollows)
            .Include(a => a.Following)
            .FirstOrDefault()!.Following.Remove(authorToUnfollow);

        await context.SaveChangesAsync();
        return true;
    }
    private async Task<Author> GetAuthor(string username)
    {
        username = username.ToUpper();
        var author = await context.Authors.FirstOrDefaultAsync(a => a.NormalizedUserName == username);
        if (author == null) throw new UserDoesNotExist();
        return author;
    }
    private async Task<bool> UserExists(string username)
    {
        username = username.ToUpper();
        return await context.Authors.AnyAsync(a => a.NormalizedUserName == username);
    }
    private async Task<bool> DoesFollow(string currentUser, string userToFollow)
    {
        currentUser = currentUser.ToUpper();
        userToFollow = userToFollow.ToUpper();
        var list = await context.Authors
            .Where(a => a.NormalizedUserName == currentUser)
            .Select(a => a.Following).FirstOrDefaultAsync();

        return list != null && list.Any(a => a.NormalizedUserName == userToFollow);
    }
    private async Task<IEnumerable<Author>> GetFollowing(string currentUser)
    {
        currentUser = currentUser.ToUpper();
        return await context.Authors
            .Where(a => a.NormalizedUserName == currentUser)
            .Select(a => a.Following).FirstOrDefaultAsync() ?? new List<Author>();
    }
    public async Task<bool> MakeFollowersUnfollow(string username)
    {
        if (!await UserExists(username)) throw new UserDoesNotExist();
        (await context.Authors.Where(a => a.NormalizedUserName == username.ToUpper())
            .Include(a => a.Followers)
            .FirstOrDefaultAsync())!.Followers.Clear();
        await context.SaveChangesAsync();
        return true;
    }
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