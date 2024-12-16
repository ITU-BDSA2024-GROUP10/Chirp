using Chirp.Core.DTO;

namespace Chirp.Core;

public interface IAuthorRepository
{
    /// <summary>
    /// Gets all authors which the given Name
    /// </summary>
    /// <param name="names"></param>
    public Task<AuthorDTO?> GetAuthorByName(string name);
    /// <summary>
    /// Gets all authors which the given Names
    /// </summary>
    /// <param name="names"></param>
    public Task<IEnumerable<AuthorDTO?>> GetAuthorsByNames(IEnumerable<string> names);
    /// <summary>
    /// Adds a new Author with the information in the AuthorDTO
    /// </summary>
    /// <param name="author"></param>
    public Task<bool> AddAuthor(AuthorDTO author);
    /// <summary>
    /// Returns the Users followed by the given User
    /// </summary>
    /// <param name="username"></param>
    public Task<IEnumerable<AuthorDTO>> GetAuthorFollows(string username);
    /// <summary>
    /// Gets the Users that follow the given User
    /// </summary>
    /// <param name="username"></param>
    public Task<IEnumerable<AuthorDTO>> GetAuthorFollowers(string username);
    /// <summary>
    /// Makes a User follow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToFollow"></param>
    public Task<bool> Follow(string currentUser, string userToFollow);
    /// <summary>
    /// Makes a User unfollow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToUnfollow"></param>
    public Task<bool> UnFollow(string currentUser, string userToUnFollow);
    /// <summary>
    /// Makes all Users which follow the given User, unfollow them
    /// </summary>
    /// <param name="user"></param>
    public Task<bool> MakeFollowersUnfollow(string user);
    /// <summary>
    /// Gets all Comments made by the given User
    /// </summary>
    /// <param name="username"></param>
    public Task<IEnumerable<CommentDTO>> GetComments(string username);
}