using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RepositoryTests.Utils;

public class InMemoryDBFixture<T> : IDisposable
    where T : DbContext
{
    private SqliteConnection Connection { get; }

    public InMemoryDBFixture()
    {
        Connection = new SqliteConnection("Filename=:memory:");
        Connection.Open();
    }

    public T GetContext()
    {
        var context = (T)Activator.CreateInstance(typeof(T), new DbContextOptionsBuilder<T>()
            .UseSqlite(Connection).Options)!;

        return context;
    }
    
    public void ResetDatabase()
    {
        using var context = GetContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}