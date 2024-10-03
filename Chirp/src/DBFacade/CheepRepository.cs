using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class CheepRepository : ICheepRepository
{
    private DbContext context = null;
    
    public async Task<IEnumerable<CheepDTO>> GetCheepsByPage(int page, int pageSize)
    {
        var query = context.Cheeps
            .Select(cheep => new {cheep.Author.Name, cheep.message, cheep.timestamp})
            .Skip((page-1)*pageSize)
            .Take(pageSize);
        
        var cheeps = await query.AsEnumerable();
        
        return cheeps.map(cheep => new CheepDTO(cheep.name, cheep.message, cheep.timestamp));
    }
    
    public async Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        var query = context.Cheeps
            .Where(cheep => cheep.Author.Name == author)
            .Select(cheep => new {cheep.name, cheep.message, cheep.timestamp})
            .Skip((page-1)*pageSize)
            .Take(pageSize);
        
        var cheeps = await query.AsEnumerable();
        
        return cheeps.map(cheep => new CheepDTO(cheep.name, cheep.message, cheep.timestamp));
    }
}