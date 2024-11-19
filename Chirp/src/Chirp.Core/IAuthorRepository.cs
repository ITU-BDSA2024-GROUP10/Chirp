using Chirp.Core.DTO;

namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<AuthorDTO?> GetAuthorByName(string name);
    
    public Task<bool> AddAuthor(AuthorDTO author);
    
    public Task<List<AuthorDTO>> GetFollows(string username);
}