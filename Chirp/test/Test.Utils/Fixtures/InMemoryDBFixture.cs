using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleDB;

namespace RepositoryTests.Fixtures;

public class InMemoryDBFixture<T> : IDisposable where T : DbContext
{
    public SqliteConnection Connection { get; private set; }

    public InMemoryDBFixture()
    {
        Connection = new SqliteConnection("Filename=:memory:");
        Connection.Open();
    }

    public T GetContext()
    {
        var builder = new DbContextOptionsBuilder<T>().UseSqlite(Connection);

        var context = (T)Activator.CreateInstance(typeof(T), builder.Options)!;
        context.Database.EnsureCreated();

        return context;
    }

    public void ResetDatabase()
    {
        using (var context = (T)Activator.CreateInstance(typeof(T), new DbContextOptionsBuilder<T>()
                   .UseSqlite(Connection).Options)!)
        {
            // Clean up all data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}