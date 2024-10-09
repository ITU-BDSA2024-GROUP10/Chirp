using Chirp.Razor.DataModels;
using SimpleDB.Model;

namespace SimpleDB;

public interface IAuthorRepository
{
    public Task<IEnumerable<AuthorDTO>> GetAuthorByName(String name); 
    public Task<AuthorDTO> GetAuthorByEmail(String email); 
}