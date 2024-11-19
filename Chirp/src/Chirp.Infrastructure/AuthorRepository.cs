using Chirp.Core;
using Chirp.Core.CustomException;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class AuthorRepository(ChirpDBContext context) : IAuthorRepository
{
    private readonly ChirpDBContext context = context;

    public async Task<AuthorDTO?> GetAuthorByName(string name)
    {
        var author = await GetAuthor(name);
        return new AuthorDTO(author.UserName!, author.Email!);
    }

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

    public async Task<List<AuthorDTO>> GetAuthorFollows(string username)
    {
        if (!await UserExists(username)) throw new UserDoesNotExist();
        return GetFollowing(username).Result.Select(a => new AuthorDTO(a.UserName!, a.Email!)).ToList();
    }

    public async Task<bool> Follow(string currentUser, string userToFollow)
    {
        if (currentUser == userToFollow) throw new ArgumentException("You cannot follow yourself");
        if (!await UserExists(currentUser)) throw new UserDoesNotExist();
        if (await DoesFollow(currentUser, userToFollow)) return false;
        
        var authorToFollow = await GetAuthor(userToFollow);
        GetAuthor(currentUser).Result.Follows.Add(authorToFollow);
        
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnFollow(string currentUser, string userToUnfollow)
    {
        if (currentUser == userToUnfollow) throw new ArgumentException("You cannot unfollow yourself");
        if (!await UserExists(currentUser)) throw new UserDoesNotExist();
        if (!await DoesFollow(currentUser, userToUnfollow)) return false;
        var authorToUnfollow = await GetAuthor(userToUnfollow);
        
        GetAuthor(currentUser).Result.Follows.Remove(authorToUnfollow);
        
        await context.SaveChangesAsync();
        return true;
    }
    
    private async Task<Author> GetAuthor(string username)
    {
        var author = await context.Authors.FirstOrDefaultAsync(a => a.UserName == username);
        if (author == null) throw new UserDoesNotExist();
        return author;
    }
    
    private async Task<bool> UserExists(string username) => 
        await context.Authors.AnyAsync(a => a.UserName == username);

    private async Task<bool> DoesFollow(string currentUser, string userToFollow)
    {
         var list = await context.Authors
                .Where(a => a.UserName == currentUser)
                .Select(a => a.Follows).FirstOrDefaultAsync();
         
         return list != null && list.Any(a => a.UserName == userToFollow);
    }

    private async Task<List<Author>> GetFollowing(string currentUser)
    {
        return await context.Authors
            .Where(a => a.UserName == currentUser)
            .Select(a => a.Follows).FirstOrDefaultAsync() ?? new List<Author>();
    }
}