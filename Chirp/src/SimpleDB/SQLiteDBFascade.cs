using System.Data.SqlClient;

namespace SimpleDB;

public class SQLiteDBFascade : IDatabaseRepository<CheepViewModel>
{
    private SqlConnection establishConnection()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CheepViewModel> GetAll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CheepViewModel> GetFromAuthor(int authorId)
    {
        throw new NotImplementedException();
    }
}