using Chirp.Core.CustomException;
using Chirp.Core.DTO;

namespace Chirp.Core;

public interface IAuthorRepository
{
    /// <summary>
    /// Gets all authors with one of the given usernames
    /// </summary>
    /// <param name="usernames"></param>
    public Task<IEnumerable<AuthorDTO?>> GetAuthorsByNames(IEnumerable<string> usernames);
    /// <summary>
    /// Gets the Users followed by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public Task<IEnumerable<AuthorDTO>> GetAuthorFollows(string username);
    /// <summary>
    /// Gets the Users that follow the given User
    /// </summary>
    /// <param name="username"></param>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public Task<IEnumerable<AuthorDTO>> GetAuthorFollowers(string username);
    /// <summary>
    /// Makes a User follow some other User
    /// </summary>
    /// <param name="userWhoFollow">username</param>
    /// <param name="userToFollow">username</param>
    /// <returns>True if the follow went through, false if there is a follow connection between the two users</returns>
    /// <exception cref="UserDoesNotExist">Thrown if a one of the users doesn't exist</exception>
    /// <exception cref="ArgumentException">Thrown if the value of <paramref name="userWhoFollow"/> is equal to <paramref name="userToFollow"/>.
    /// </exception>
    public Task<bool> Follow(string userWhoFollow, string userToFollow);
    /// <summary>
    /// Makes a User unfollow some other User
    /// </summary>
    /// <param name="userWhoFollows">username</param>
    /// <param name="userToUnFollow">username</param>
    /// <returns>True if the unfollow went through, false if there isn't a follow connection between the two users</returns>
    /// <exception cref="UserDoesNotExist">Thrown if a one of the users doesn't exist</exception>
    /// <exception cref="ArgumentException">Thrown if the value of <paramref name="userWhoFollows"/> is equal to <paramref name="userToUnFollow"/>.
    /// </exception>
    public Task<bool> UnFollow(string userWhoFollows, string userToUnFollow);
    /// <summary>
    /// Makes all Users which follow the given User, unfollow them
    /// </summary>
    /// <param name="username"></param>
    /// <returns>True if the followers unfollowed</returns>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public Task<bool> MakeFollowersUnfollow(string username);
    /// <summary>
    /// Gets all Comments made by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public Task<IEnumerable<CommentDTO>> GetComments(string username);
}