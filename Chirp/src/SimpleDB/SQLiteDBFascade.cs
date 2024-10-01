using Microsoft.Data.Sqlite;
using SimpleDB.Model;

namespace SimpleDB;

public class SQLiteDBFascade : IDatabaseRepository<CheepViewModel>
{
    private readonly string _connectionString =
        "C:\\Users\\chris\\Desktop\\Pogramerings projekter\\Fælles over github\\Chirp\\Chirp\\data\\chirp.db";

    private SqliteConnection establishConnection()
    {
        var connection = new SqliteConnection($"Data Source={_connectionString}");
        connection.Open();

        return connection;
    }

    private IEnumerable<CheepViewModel> ReadCheeps(SqliteCommand command)
    {
        List<CheepViewModel> result = new List<CheepViewModel>();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var cheep = new CheepViewModel(reader.GetString(0), reader.GetString(1), reader.GetString(2));
            result.Add(cheep);
        }

        return result;
    }

    public IEnumerable<CheepViewModel> GetAll()
    {
        var query = """
                    SELECT u.username, m.text, m.pub_date FROM message m
                    JOIN user u ON m.author_id = u.user_id;
                    """;
        using (var connection = establishConnection())
        {
            var command = connection.CreateCommand();
            command.CommandText = query;

            return ReadCheeps(command);
        }
    }

    public IEnumerable<CheepViewModel> GetFromAuthor(String author)
    {
        var query = """
                    SELECT u.username, m.text, m.pub_date FROM message m
                    JOIN user u ON m.author_id = u.user_id
                    WHERE u.username = @author;
                    """;

        using (var connection = establishConnection())
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@author", author);

            return ReadCheeps(command);
        }
    }
}