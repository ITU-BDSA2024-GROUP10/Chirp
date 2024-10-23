using SimpleDB.DTO;

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    public Task<AuthorDTO?> GetAuthorByName(string name); 
    public Task<AuthorDTO> GetAuthorByEmail(string email); 
    
    public Task<bool> AddAuthor(AuthorDTO author);
}