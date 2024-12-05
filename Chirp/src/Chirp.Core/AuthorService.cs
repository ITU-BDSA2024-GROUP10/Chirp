using Chirp.Core.DTO;

namespace Chirp.Core;

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