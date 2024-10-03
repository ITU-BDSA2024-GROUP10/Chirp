using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class CheepRepository : ICheepRepository
{
    private DbContext context = null;
    
    public Task<IEnumerable<CheepDTO>> GetCheepsByPage(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
    
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(String author, int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}