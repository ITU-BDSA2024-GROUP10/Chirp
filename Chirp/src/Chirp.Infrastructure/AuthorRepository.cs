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

    public async Task<List<AuthorDTO>> GetFollows(string username)
    {
        return await context.Authors
            .Where((a => a.UserName == username))
            .Select(a => a.Follows)
            .Select(follows => follows.Select(author => new AuthorDTO(author.UserName!, author.Email!)).ToList())
            .FirstOrDefaultAsync() ?? new List<AuthorDTO>();
    }
}