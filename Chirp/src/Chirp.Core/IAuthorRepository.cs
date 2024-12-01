using Chirp.Core.DTO;

namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<AuthorDTO?> GetAuthorByName(string name);
    public Task<bool> AddAuthor(AuthorDTO author);
    public Task<List<AuthorDTO>> GetAuthorFollows(string username);
    public Task<List<AuthorDTO>> GetAuthorFollowers(string username);
    public Task<bool> Follow(string currentUser, string userToFollow);
    public Task<bool> UnFollow(string currentUser, string userToUnFollow);
    public Task<bool> MakeFollowersUnfollow(string user);
}