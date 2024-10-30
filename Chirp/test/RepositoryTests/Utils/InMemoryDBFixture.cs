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
        
        // Clean up all data
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}