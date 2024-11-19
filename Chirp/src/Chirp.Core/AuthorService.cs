using Chirp.Core.DTO;

namespace Chirp.Core;


public interface IAuthorService
{
    public List<AuthorDTO> GetFollows(string username);
}
public class AuthorService(IAuthorRepository db) : IAuthorService
{
    public List<AuthorDTO> GetFollows(string username)
    {
        return db.GetAuthorFollows(username).Result;
    }
}