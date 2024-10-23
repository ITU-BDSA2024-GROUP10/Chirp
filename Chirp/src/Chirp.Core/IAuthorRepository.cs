using Chirp.Core.DTO;

namespace Chirp.Core;

public interface IAuthorRepository
{
    public Task<AuthorDTO?> GetAuthorByName(string name);
    public Task<AuthorDTO?> GetAuthorByEmail(string email);

    public Task<bool> AddAuthor(AuthorDTO author);
}