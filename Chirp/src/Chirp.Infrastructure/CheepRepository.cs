using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepRepository(ChirpDBContext context) : ICheepRepository
{
    private readonly ChirpDBContext context = context;

    public async Task<IEnumerable<CheepDTO>?> GetCheepsByPage(int page, int pageSize)
    {
        if (pageSize < 0) return null;
            
        var query = context.Cheeps
            .Select(cheep => new { cheep.Author.UserName, cheep.Message, cheep.TimeStamp })
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var cheeps = await query.ToListAsync();

        return cheeps.Select(cheep =>
            new CheepDTO(cheep.UserName!, cheep.Message, new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return await GetCheepsFromAuthorsByPage([author], page, pageSize);
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthor(string author)
    {
        author = author.ToUpper();
        var query = context.Cheeps
            .Where(cheep => cheep.Author.NormalizedUserName == author)
            .Select(cheep => new { cheep.Author.UserName, cheep.Message, cheep.TimeStamp })
            .OrderByDescending(cheep => cheep.TimeStamp);
        var cheeps = await query.ToListAsync();

        return cheeps.Select(cheep =>
            new CheepDTO(cheep.UserName!, cheep.Message, new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }
    
    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorsByPage(IEnumerable<string> authors, int page, int pageSize)
    {
        authors = authors.Select(author => author.ToUpper());
        var query = context.Cheeps
            .Where(cheep => authors.Contains(cheep.Author.NormalizedUserName!))
            .Select(cheep => new { cheep.Author.UserName, cheep.Message, cheep.TimeStamp })
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var cheeps = await query.ToListAsync();

        return cheeps.Select(cheep =>
            new CheepDTO(cheep.UserName!, cheep.Message, new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }

    public async Task<bool> CreateCheep(CheepDTO cheep)
    {
        var author = await context.Authors
            .Where(a => a.UserName == cheep.Author)
            .FirstOrDefaultAsync();
        if (author == null) return false;
        var cheep2 = new Cheep {Author = author, Message = cheep.Message, TimeStamp = DateTimeOffset.FromUnixTimeSeconds(cheep.UnixTimestamp).DateTime};
        context.Cheeps.Add(cheep2);
        await context.SaveChangesAsync();
        return true;
    }
}