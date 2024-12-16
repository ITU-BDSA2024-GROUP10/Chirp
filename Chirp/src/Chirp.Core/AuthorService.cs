using Chirp.Core.DTO;

namespace Chirp.Core;
/// <summary>
/// The IAuthorService defines the methods That the AuthorService must implement 
/// </summary>
public interface IAuthorService
{
    public IEnumerable<AuthorDTO> GetFollows(string username);
    public IEnumerable<AuthorDTO> GetFollowers(string username);
    public bool Follow(string currentUser, string userToFollow);
    public bool Unfollow(string currentUser, string userToUnFollow);
    public bool MakeFollowersUnfollow(string username);
    public IEnumerable<CommentDTO> GetComments(string username);
    public IEnumerable<AuthorDTO?> GetAuthorsByNames(IEnumerable<String> names);
}
/// <summary>
/// The AuthorServices handles calls to the AuthorRepository from the UI
/// </summary>
/// <param name="db"></param>
public class AuthorService(IAuthorRepository db) : IAuthorService
{
    /// <summary>
    /// Returns the Users followed by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public IEnumerable<AuthorDTO> GetFollows(string username)
    {
        return db.GetAuthorFollows(username).Result;
    }
    /// <summary>
    /// Gets the Users that follow the given User
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public IEnumerable<AuthorDTO> GetFollowers(string username)
    {
        return db.GetAuthorFollowers(username).Result;
    }
    /// <summary>
    /// Makes all Users which follow the given User, unfollow them
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public bool MakeFollowersUnfollow(string username)
    {
        return db.MakeFollowersUnfollow(username).Result;
    }
    /// <summary>
    /// Gets all Comments made by the given User
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public IEnumerable<CommentDTO> GetComments(string username)
    {
        return db.GetComments(username).Result;
    }
    
    /// <summary>
    /// Makes a User follow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToFollow"></param>
    /// <returns></returns>
    public bool Follow(string currentUser, string userToFollow)
    {
        return db.Follow(currentUser, userToFollow).Result;
    }
    /// <summary>
    /// Makes a User unfollow some other User
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="userToUnFollow"></param>
    /// <returns></returns>
    public bool Unfollow(string currentUser, string userToUnFollow)
    {
        return db.UnFollow(currentUser, userToUnFollow).Result;
    }
    /// <summary>
    /// Gets all authors which the given Names
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public IEnumerable<AuthorDTO?> GetAuthorsByNames(IEnumerable<String> names)
    {
        return db.GetAuthorsByNames(names).Result;
    }
}