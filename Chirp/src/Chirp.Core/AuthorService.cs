using Chirp.Core.DTO;

namespace Chirp.Core;


public interface IAuthorService
{
    public List<AuthorDTO> GetFollows(string username);
    public List<AuthorDTO> GetFollowers(string username);
    public bool Follow(string currentUser, string userToFollow);
    public bool Unfollow(string currentUser, string userToUnFollow);
    public bool MakeFollowersUnfollow(string username);
}
public class AuthorService(IAuthorRepository db) : IAuthorService
{
    public List<AuthorDTO> GetFollows(string username)
    {
        return db.GetAuthorFollows(username).Result;
    }
    
    public List<AuthorDTO> GetFollowers(string username)
    {
        return db.GetAuthorFollowers(username).Result;
    }

    public bool MakeFollowersUnfollow(string username)
    {
        return db.MakeFollowersUnfollow(username).Result;
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