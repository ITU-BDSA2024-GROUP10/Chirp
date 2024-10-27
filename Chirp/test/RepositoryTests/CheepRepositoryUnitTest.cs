using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RepositoryTests;

public class CheepRepositoryUnitTest : IDisposable
{
    private readonly SqliteConnection _connection;

    public CheepRepositoryUnitTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
    }

    private ChirpDBContext GetContext()
    {
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection);

        var context = new ChirpDBContext(builder.Options);
        context.Database.EnsureDeleted(); //Ensures that any test runs on their own DB
        context.Database.EnsureCreated(); // Applies the schema to the database

        return context;
    }

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsCorrectCheepWhenMultipleAuthorsInDB()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ChirpDBContext(options))
        {
            context.Database.EnsureCreated();

            List<Author> authors = [];
            List<Cheep> cheeps = [];

            for (int i = 0; i < 5; i++)
            {
                authors.Add(new Author { Name = $"name{i}", Email = $"{i}@mail.com", Cheeps = [] });
                cheeps.Add(new Cheep { Author = authors.ElementAt(i), Message = $"test{i}", TimeStamp = DateTime.Now });
            }

            context.Authors.AddRange(authors);
            context.Cheeps.AddRange(cheeps);
            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);
            for (int i = 0; i < 5; i++)
            {
                var result = await CheepRepo.GetCheepsFromAuthorByPage(authors[i].Name, 1, 1);
                var resultDTO = result.ToList().ElementAt(0);

                Assert.Equal(cheeps[i].Author.Name, resultDTO.Author);
                Assert.Equal(cheeps[i].Message, resultDTO.Message);
                Assert.Equal(((DateTimeOffset)cheeps[i].TimeStamp).ToUnixTimeSeconds(), resultDTO.UnixTimestamp);
            }
        }
    }

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsNoCheepsForNonexistentAuthor()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ChirpDBContext(options))
        {
            context.Database.EnsureCreated();

            List<Author> authors = [];
            List<Cheep> cheeps = [];

            for (int i = 0; i < 5; i++)
            {
                authors.Add(new Author { Name = $"name{i}", Email = $"{i}@mail.com", Cheeps = [] });
                cheeps.Add(new Cheep { Author = authors.ElementAt(i), Message = $"test{i}", TimeStamp = DateTime.Now });
            }

            context.Authors.AddRange(authors);
            context.Cheeps.AddRange(cheeps);
            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);

            var result = await CheepRepo.GetCheepsFromAuthorByPage("Bill", 1, 5);
            Assert.Empty(result.ToList());
        }
    }

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsAllCheepsFromAuthorForLargeNumberOfCheepsOnMultiplePages()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ChirpDBContext(options))
        {
            context.Database.EnsureCreated();

            Author AuthorA = new Author { Name = "Bill", Email = "Bill@email.com", Cheeps = [] };
            Author AuthorB = new Author { Name = "Amy", Email = "Amy@email.com", Cheeps = [] };
            int authTotal = 0;
            var rand = new Random();

            for (int i = 0; i < 100; i++)
            {
                int r = rand.Next(2);
                if (r == 1)
                {
                    context.Cheeps.Add(new Cheep { Author = AuthorB, Message = "", TimeStamp = DateTime.Now });
                }
                else
                {
                    context.Cheeps.Add(new Cheep { Author = AuthorA, Message = "", TimeStamp = DateTime.Now });
                    authTotal++;
                }
            }

            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);

            int pageNo = 1;
            int totalCount = 0;
            while (true)
            {
                var result = await CheepRepo.GetCheepsFromAuthorByPage("Bill", pageNo, 20);
                var count = result.ToList().Count;
                if (count == 0)
                    break;
                totalCount += count;
                pageNo++;
            }

            Assert.Equal(authTotal, totalCount);
        }
    }

    [Fact]
    public async Task CreateCheep()
    {
        //arrange
        var chirpContext = GetContext();
        var author = new Author { Name = "John Doe", Email = "JohnDoe@gmail.com" };
        chirpContext.Authors.Add(author);
        chirpContext.SaveChanges();

        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var newCheep = new CheepDTO("John Doe", "message", unixTimestamp);

        ICheepRepository repo = new CheepRepository(chirpContext);

        //act
        var result = await repo.CreateCheep(newCheep);

        //Assert
        Assert.True(result);
        
        var containsCheep = chirpContext.Cheeps.Any(cheep =>
            cheep.Author.Name == "John Doe" && cheep.Message == "message" &&
            ((DateTimeOffset)cheep.TimeStamp).ToUnixTimeSeconds() == unixTimestamp);
        Assert.True(containsCheep);
    }
}