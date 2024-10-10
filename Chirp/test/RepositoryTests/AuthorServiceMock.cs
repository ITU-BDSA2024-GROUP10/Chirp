using SimpleDB;
using SimpleDB.Model;
namespace RepositoryTests;

//This code has been made in collaboration with ChatGPT
public class AuthorServiceMock(IAuthorRepository authorRepository)
{
    public async Task<bool> RegisterAuthor(string name, string email)
    {
        // Check if author already exists by name or email
        var existingAuthorByName = await authorRepository.GetAuthorByName(name);
        var existingAuthorByEmail = await authorRepository.GetAuthorByEmail(email);

        if (existingAuthorByName != null || existingAuthorByEmail != null)
        {
            return false; // Author already exists
        }

        // If not exists, add new author
        var newAuthor = new AuthorDTO(name, email);
        return await authorRepository.AddAuthor(newAuthor);
    }
}