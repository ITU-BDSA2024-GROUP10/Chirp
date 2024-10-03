using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class SQLiteDBFascade : IDatabaseRepository<CheepDTO>
{
    private DbContext context = null;
    
    public IEnumerable<CheepDTO> GetByPage(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
    
    public IEnumerable<CheepDTO> GetFromAuthorByPage(String author, int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}