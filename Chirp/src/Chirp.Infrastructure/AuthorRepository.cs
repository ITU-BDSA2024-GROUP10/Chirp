using Chirp.Razor.DataModels;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class AuthorRepository : IAuthorRepository
{
    private ChirpDBContext context;

    public AuthorRepository(ChirpDBContext context)
    {
        this.context = context;
    }

    public async Task<AuthorDTO?> GetAuthorByName(string name) =>
        await context.Authors
            .Where(a => a.Name == name)
            .Select(a => new AuthorDTO(a.Name, a.Email))
            .FirstOrDefaultAsync();

    public async Task<AuthorDTO?> GetAuthorByEmail(string email) =>
        await context.Authors
            .Where(a => a.Email == email)
            .Select(a => new AuthorDTO(a.Name, a.Email))
            .FirstOrDefaultAsync();


    public Task<bool> AddAuthor(AuthorDTO author)
    {
        if (context.Authors.Any(a => a.Name == author.Name)) return Task.FromResult(false);
        if (context.Authors.Any(a => a.Email == author.Email)) return Task.FromResult(false);
        
        context.Authors.Add(new Author
        {
            Name = author.Name,
            Email = author.Email
        });

        context.SaveChanges();

        return Task.FromResult(true);
    }
}