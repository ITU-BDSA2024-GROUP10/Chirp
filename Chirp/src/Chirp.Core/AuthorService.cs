using Chirp.Core.CustomException;
using Chirp.Core.DTO;

namespace Chirp.Core;

/// <summary>
/// The IAuthorService defines what information you can get from an Author 
/// </summary>
public interface IAuthorService
{
    /// <summary>
    /// Gets the Users followed by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public IEnumerable<AuthorDTO> GetFollows(string username);

    /// <summary>
    /// Gets the Users that follow the given User
    /// </summary>
    /// <param name="username"></param>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public IEnumerable<AuthorDTO> GetFollowers(string username);

    /// <summary>
    /// Makes a User follow some other User
    /// </summary>
    /// <param name="userWhoFollow">username</param>
    /// <param name="userToFollow">username</param>
    /// <returns>True if the follow went through, false if there already exists a follow connection between the two users</returns>
    /// <exception cref="UserDoesNotExist">Thrown if one of the users doesn't exist</exception>
    /// <exception cref="ArgumentException">Thrown if the value of <paramref name="userWhoFollow"/> is equal to <paramref name="userToFollow"/>.
    /// </exception>
    public bool Follow(string userWhoFollow, string userToFollow);

    /// <summary>
    /// Makes a User unfollow some other User
    /// </summary>
    /// <param name="userWhoFollows">username</param>
    /// <param name="userToUnFollow">username</param>
    /// <returns>True if the unfollow went through, false if there isn't a follow connection between the two users</returns>
    /// <exception cref="UserDoesNotExist">Thrown if one of the users doesn't exist</exception>
    /// <exception cref="ArgumentException">Thrown if the value of <paramref name="userWhoFollows"/> is equal to <paramref name="userToUnFollow"/>.
    /// </exception>
    public bool Unfollow(string userWhoFollows, string userToUnFollow);

    /// <summary>
    /// Makes all Users which follow the given User, unfollow them
    /// </summary>
    /// <param name="username"></param>
    /// <returns>True if the followers unfollowed</returns>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public bool MakeFollowersUnfollow(string username);

    /// <summary>
    /// Gets all Comments made by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <exception cref="UserDoesNotExist">Thrown if a user with the given username doesn't exist</exception>
    public IEnumerable<CommentDTO> GetComments(string username);

    /// <summary>
    /// Gets all authors with one of the given usernames
    /// </summary>
    /// <param name="usernames"></param>
    public IEnumerable<AuthorDTO?> GetAuthorsByNames(IEnumerable<String> usernames);
}

public class AuthorService(IAuthorRepository db) : IAuthorService
{
    public IEnumerable<AuthorDTO> GetFollows(string username)
    {
        return db.GetAuthorFollows(username).Result;
    }

    public IEnumerable<AuthorDTO> GetFollowers(string username)
    {
        return db.GetAuthorFollowers(username).Result;
    }

    public bool MakeFollowersUnfollow(string username)
    {
        return db.MakeFollowersUnfollow(username).Result;
    }

    public IEnumerable<CommentDTO> GetComments(string username)
    {
        return db.GetComments(username).Result;
    }

    public bool Follow(string userWhoFollow, string userToFollow)
    {
        return db.Follow(userWhoFollow, userToFollow).Result;
    }

    public bool Unfollow(string userWhoFollows, string userToUnFollow)
    {
        return db.UnFollow(userWhoFollows, userToUnFollow).Result;
    }

    public IEnumerable<AuthorDTO?> GetAuthorsByNames(IEnumerable<String> usernames)
    {
        return db.GetAuthorsByNames(usernames).Result;
    }
}