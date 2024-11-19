using Chirp.Core.DTO;

namespace Chirp.Core;


public interface IAuthorService
{
    public List<AuthorDTO> GetFollows(string username);
    public bool Follow(string currentUser, string userToFollow);
    public bool Unfollow(string currentUser, string userToUnFollow);
}
public class AuthorService(IAuthorRepository db) : IAuthorService
{
    public List<AuthorDTO> GetFollows(string username)
    {
        return db.GetAuthorFollows(username).Result;
    }
    
    public bool Follow(string currentUser, string userToFollow)
    {
        return db.Follow(currentUser, userToFollow).Result;
    }
    
    public bool Unfollow(string currentUser, string userToUnFollow)
    {
        return db.UnFollow(currentUser, userToUnFollow).Result;
    }
}