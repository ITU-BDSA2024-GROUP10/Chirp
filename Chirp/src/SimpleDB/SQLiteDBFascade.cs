using System.Collections;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace SimpleDB;

public class SQLiteDBFascade : IDatabaseRepository<CheepViewModel>
{
    private readonly string _connectionString = "./data/chirp.db";
    private SqliteConnection establishConnection()
    {
        var connection = new SqliteConnection($"Data Source={_connectionString}");
        connection.Open();
        
        return connection; 
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