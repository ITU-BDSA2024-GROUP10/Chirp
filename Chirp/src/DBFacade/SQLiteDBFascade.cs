using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class SQLiteDBFascade : IDatabaseRepository<CheepViewModel>
{
    
    public IEnumerable<CheepViewModel> GetByPage(int page, int pageSize)
    {
    }
    
    public IEnumerable<CheepViewModel> GetFromAuthorByPage(String author, int page, int pageSize)
    {
    }
}