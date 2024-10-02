using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class SQLiteDBFascade : IDatabaseRepository<CheepViewModel>
{
    private DbContext context = null;
    
    public IEnumerable<CheepViewModel> GetByPage(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
    
    public IEnumerable<CheepViewModel> GetFromAuthorByPage(String author, int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}