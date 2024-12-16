using Chirp.Core.DTO;

namespace Chirp.Core;
/// <summary>
/// The IAuthorService defines the methods That the AuthorService must implement 
/// </summary>
public interface IAuthorService
{
    /// <summary>
    /// Returns the Users followed by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public IEnumerable<AuthorDTO> GetFollows(string username);
    /// <summary>
    /// Gets the Users that follow the given User
    /// </summary>
    /// <param name="username"></param>
    public IEnumerable<AuthorDTO> GetFollowers(string username);
    /// <summary>
    /// Makes a User follow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToFollow"></param>
    public bool Follow(string currentUser, string userToFollow);
    /// <summary>
    /// Makes a User unfollow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToUnFollow"></param>
    public bool Unfollow(string currentUser, string userToUnFollow);
    /// <summary>
    /// Makes all Users which follow the given User, unfollow them
    /// </summary>
    /// <param name="username"></param>
    public bool MakeFollowersUnfollow(string username);
    /// <summary>
    /// Gets all Comments made by the given User
    /// </summary>
    /// <param name="username"></param>
    public IEnumerable<CommentDTO> GetComments(string username);
    /// <summary>
    /// Gets all authors which the given Names
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public IEnumerable<AuthorDTO?> GetAuthorsByNames(IEnumerable<String> names);
}
/// <summary>
/// The AuthorServices handles calls to the AuthorRepository from the UI
/// </summary>
/// <param name="db"></param>
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
    public bool Follow(string currentUser, string userToFollow)
    {
        return db.Follow(currentUser, userToFollow).Result;
    }
    public bool Unfollow(string currentUser, string userToUnFollow)
    {
        return db.UnFollow(currentUser, userToUnFollow).Result;
    }
    public IEnumerable<AuthorDTO?> GetAuthorsByNames(IEnumerable<String> names)
    {
        return db.GetAuthorsByNames(names).Result;
    }
}