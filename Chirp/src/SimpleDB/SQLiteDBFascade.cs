using System.Data.SqlClient;

namespace SimpleDB;

public class SQLiteDBFascade : IDatabaseRepository<Cheep>
{
    private SqlConnection establishConnection()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Cheep> GetAll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Cheep> GetFromAuthor(int authorId)
    {
        throw new NotImplementedException();
    }
}