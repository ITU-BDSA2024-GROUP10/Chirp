using Chirp.Core.DTO;

namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<AuthorDTO?> GetAuthorByName(string name);
    public Task<IEnumerable<AuthorDTO?>> GetAuthorsByNames(IEnumerable<string> names);
    public Task<bool> AddAuthor(AuthorDTO author);
    public Task<IEnumerable<AuthorDTO>> GetAuthorFollows(string username);
    public Task<IEnumerable<AuthorDTO>> GetAuthorFollowers(string username);
    public Task<bool> Follow(string currentUser, string userToFollow);
    public Task<bool> UnFollow(string currentUser, string userToUnFollow);
    public Task<bool> MakeFollowersUnfollow(string user);
    public Task<IEnumerable<CommentDTO>> GetComments(string username);
}