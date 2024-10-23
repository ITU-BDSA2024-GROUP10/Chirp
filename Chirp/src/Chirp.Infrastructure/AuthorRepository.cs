using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using SimpleDB;

namespace Chirp.Infrastructure;

public class AuthorRepository(ChirpDBContext context) : IAuthorRepository
{
    private readonly ChirpDBContext context = context;

    public async Task<AuthorDTO?> GetAuthorByName(string name) =>
        await context.Authors
            .Where(a => a.Name == name)
            .Select(a => new AuthorDTO(a.Name, a.Email))
            .FirstOrDefaultAsync();

    public async Task<AuthorDTO?> GetAuthorByEmail(string email)
    {
        var query = context.Authors
            .Where(author => author.Email == email)
            .Select(author => new {author.Name, author.Email});
            
        
        
        var authors = await query.ToListAsync();
        
        return authors.Select(author => new AuthorDTO(author.Name, author.Email)).FirstOrDefault();
    }

    public async Task<bool> AddAuthor(AuthorDTO author)
    {
        var newAuthor = new Author
        {
            Name = author.Name,
            Email = author.Email
        };

        try
        {
            context.Authors.Add(newAuthor);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}