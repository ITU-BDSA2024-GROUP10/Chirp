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
        var query = @"SELECT u.username, m.text, m.pup_date FROM message m
                        JOIN user u ON m.author_id = u.user_id";
        using (var connection = establishConnection())
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            
            List<CheepViewModel> result = new List<CheepViewModel>();
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var cheep = new CheepViewModel(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                result.Add(cheep);
            }
            return result;
        }
    }

    public IEnumerable<CheepViewModel> GetFromAuthor(int authorId)
    {
        throw new NotImplementedException();
    }
}