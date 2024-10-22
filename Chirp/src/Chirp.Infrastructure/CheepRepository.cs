using System.Runtime.Serialization;
using Chirp.Razor.DataModels;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class CheepRepository : ICheepRepository
{
    private ChirpDBContext context;

    public CheepRepository(ChirpDBContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<CheepDTO>> GetCheepsByPage(int page, int pageSize) =>
        await context.Cheeps
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(cheep => new CheepDTO(cheep.Author.Name, cheep.Message, cheep.TimeStamp))
            .ToListAsync();

    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(string author, int page, int pageSize) =>
        await context.Cheeps
            .Where(cheep => cheep.Author.Name == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(cheep => new CheepDTO(cheep.Author.Name, cheep.Message, cheep.TimeStamp))
            .ToListAsync();

    public Task<bool> CreateCheep(CheepDTO cheep)
    {
        var author = context.Authors.FirstOrDefault(a => a.Name == cheep.Author);
        if (author == null) return Task.FromResult(false);

        context.Cheeps.Add(new Cheep
        {
            Author = author,
            Message = cheep.Message,
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(cheep.UnixTimestamp).DateTime
        });

        context.SaveChanges();

        return Task.FromResult(true);
    }
}