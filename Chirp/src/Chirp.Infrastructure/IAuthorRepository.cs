using Chirp.Razor.DataModels;
using SimpleDB.Model;

namespace SimpleDB;

public interface IAuthorRepository
{
    public Task<AuthorDTO> GetAuthorByName(string name); 
    public Task<AuthorDTO> GetAuthorByEmail(string email); 
}