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
    
    public async Task<IEnumerable<CheepDTO>> GetCheepsByPage(int page, int pageSize)
    {
        var query = context.Cheeps
            .Select(cheep => new {cheep.Author.Name, cheep.Message, cheep.TimeStamp})
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page-1)*pageSize)
            .Take(pageSize);
        
        
        var cheeps = await query.ToListAsync();
        
        return cheeps.Select(cheep => new CheepDTO(cheep.Name, cheep.Message, new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }
    
    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        var query = context.Cheeps
            .Where(cheep => cheep.Author.Name == author)
            .Select(cheep => new {cheep.Author.Name, cheep.Message, cheep.TimeStamp})
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page-1)*pageSize)
            .Take(pageSize);
        
        var cheeps = await query.ToListAsync();
        
        return cheeps.Select(cheep => new CheepDTO(cheep.Name, cheep.Message, new DateTimeOffset(cheep.TimeStamp).ToUnixTimeSeconds()));
    }

    public void CreateCheep(CheepDTO cheep)
    {
        throw new NotImplementedException();
    }
}