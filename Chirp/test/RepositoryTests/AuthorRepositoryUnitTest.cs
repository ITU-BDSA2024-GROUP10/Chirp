using Chirp.Razor.DataModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleDB;

namespace RepositoryTests;

public class AuthorRepositoryUnitTest
{
    [Fact]
    public async void GetAuthorByEmailTest()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        
        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        IAuthorRepository authorRepository = new AuthorRepository(context);

        var author = new Author
        {
            Name = "Tester",
            Email = "Tester@tmail.com"
        };

        context.Authors.Add(author);
        await context.SaveChangesAsync();
        
        var returnAuthor = await authorRepository.GetAuthorByEmail("Tester@tmail.com");
        Assert.NotNull(returnAuthor);
        Assert.Equal("Tester", returnAuthor.Name);
        Assert.Equal("Tester@tmail.com", returnAuthor.Email);
    }
}