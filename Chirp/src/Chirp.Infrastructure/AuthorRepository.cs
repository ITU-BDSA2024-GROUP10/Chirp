using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class AuthorRepository(ChirpDBContext context) : IAuthorRepository
{
    private readonly ChirpDBContext context = context;

    public async Task<AuthorDTO?> GetAuthorByName(string name) =>
        await context.Authors
            .Where(a => a.UserName == name)
            .Select(a => new AuthorDTO(a.UserName!, a.Email!))
            .FirstOrDefaultAsync();

    public async Task<AuthorDTO?> GetAuthorByEmail(string email)
    {
        var query = context.Authors
            .Where(author => author.Email == email)
            .Select(author => new { author.Name, author.Email });


        var authors = await query.ToListAsync();

        return authors.Select(author => new AuthorDTO(author.Name, author.Email ?? string.Empty)).FirstOrDefault();
    }

    public async Task<bool> AddAuthor(AuthorDTO author)
    {
        try
        {
            context.Authors.Add(Author.CreateAuthor(author));
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}