using Chirp.Razor.DataModels;

namespace SimpleDB;

public interface IAuthorRepository
{
    public Task<IEnumerable<Author>> GetAuthorByName(String name); 
}