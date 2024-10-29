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

    #region get cheeps

    [Theory]
    [InlineData(1, 3, 3)] //from start
    [InlineData(2, 3, 3)] //random section from middle
    [InlineData(3, 3, 1)] //last page with fewer cheeps
    [InlineData(1, 7, 7)] //all cheeps
    [InlineData(1, 10, 7)] //more than all cheeps
    [InlineData(0, 3, 3)] //page 0
    [InlineData(-1, 3, 3)] //negative page
    [InlineData(1, 0, 0)] //pagesize 0
    [InlineData(4, 3, 0)] //pagesize * pageno > no of cheeps
    public async Task GetCheepsByPage_ReturnsCorrectNumberOfCheeps(int page, int pageSize, int? expected)
    {
        //arrange
        var chirpContext = GetContext();
        var author = new Author { Id = "1", Name = "Bill", Email = "Bill@email", Cheeps = [] };
        for (int i = 0; i < 7; i++)
        {
            var cheep = new Cheep { Author = author, Message = $"test_{i}", TimeStamp = DateTime.Now };
            chirpContext.Cheeps.Add(cheep);
        }

        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);

        //act
        var result = await cheepRepo.GetCheepsByPage(page, pageSize);

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.Count());
    }

    [Fact]
    public async Task GetCheepsByPage_NegativePageSize_ReturnsNull()
    {
        //arrange
        var chirpContext = GetContext();
        var author = new Author { Name = "mr. test", Email = "test@test.test" };
        var cheep = new Cheep { Author = author, Message = "test", TimeStamp = DateTime.Now };
        chirpContext.Cheeps.Add(cheep);
        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);

        //act
        var result = await cheepRepo.GetCheepsByPage(1, -1);

        //assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCheepsByPage_ReturnsCorrectCheepsFromMiddlePages()
    {
        //arrange
        var chirpContext = GetContext();
        var author = new Author { Id = "1", Name = "Bill", Email = "Bill@email", Cheeps = [] };
        List<Cheep> cheeps = [];
        for (int i = 0; i < 7; i++)
        {
            var cheep = new Cheep { Author = author, Message = $"test_{i}", TimeStamp = DateTime.Now };
            cheeps.Add(cheep);
        }

        cheeps.Reverse();
        chirpContext.Cheeps.AddRange(cheeps);
        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);

        //act
        var result = await cheepRepo.GetCheepsByPage(2, 3);

        //assert
        Assert.NotNull(result);

        var resultList = result.ToList();
        for (int i = 0; i < 3; i++)
        {
            var expectedCheep = cheeps.ElementAt(i + 3);
            var singleCheep = resultList.ElementAt(i);

            Assert.Equal(expectedCheep.Author.Name, singleCheep.Author);
            Assert.Equal(expectedCheep.Message, singleCheep.Message);
            Assert.Equal(((DateTimeOffset)expectedCheep.TimeStamp).ToUnixTimeSeconds(), singleCheep.UnixTimestamp);
        }
    }

    [Theory]
    [InlineData(3, 3)]
    public async Task GetCheepsByPage_ReturnsNoCheepsForEmptyRepository(int page, int pageSize)
    {
        //arrange
        var chirpContext = GetContext();
        var cheepRepo = new CheepRepository(chirpContext);

        //act
        var result = await cheepRepo.GetCheepsByPage(page, pageSize);

        //assert
        Assert.NotNull(result);
        Assert.Empty(result.ToList());
    }

    [Fact]
    public async Task GetCheepsByPage_ReturnsCorrectSingleCheep()
    {
        //arrange
        var chirpContext = GetContext();
        var author = new Author { Id = "1", Name = "Bill", Email = "Bill@email.com", Cheeps = [] };
        var timeStamp = new DateTime(2000, 01, 01);
        var cheep = new Cheep { Author = author, Message = "test", TimeStamp = timeStamp };
        chirpContext.Cheeps.Add(cheep);
        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);

        //act
        var result = await cheepRepo.GetCheepsByPage(1, 1);

        //assert
        Assert.NotNull(result);

        var resultList = result.ToList();
        var expected = new CheepDTO("Bill", "test", ((DateTimeOffset)timeStamp).ToUnixTimeSeconds());
        Assert.Single(resultList);

        var singleCheep = resultList.ElementAt(0);
        Assert.Equal(expected.Author, singleCheep.Author);
        Assert.Equal(expected.Message, singleCheep.Message);
        Assert.Equal(expected.UnixTimestamp, singleCheep.UnixTimestamp);
    }

    #endregion

    #region get by author

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsCorrectCheepWhenMultipleAuthorsInDB()
    {
        //arrange
        var chirpContext = GetContext();
        List<Author> authors = [];
        List<Cheep> cheeps = [];

        for (int i = 0; i < 5; i++)
        {
            var author = new Author { Name = $"name{i}", Email = $"{i}@mail.com", Cheeps = [] };
            var cheep = new Cheep { Author = authors.ElementAt(i), Message = $"test{i}", TimeStamp = DateTime.Now };
            authors.Add(author);
            cheeps.Add(cheep);
        }

        chirpContext.Authors.AddRange(authors);
        chirpContext.Cheeps.AddRange(cheeps);
        await chirpContext.SaveChangesAsync();

        var cheepRepo = new CheepRepository(chirpContext);
        for (int i = 0; i < 5; i++)
        {
            //act
            var result = await cheepRepo.GetCheepsFromAuthorByPage(authors[i].Name, 1, 1);
            var resultDTO = result.ToList().ElementAt(0);

            //arrange
            Assert.Equal(cheeps[i].Author.Name, resultDTO.Author);
            Assert.Equal(cheeps[i].Message, resultDTO.Message);
            Assert.Equal(((DateTimeOffset)cheeps[i].TimeStamp).ToUnixTimeSeconds(), resultDTO.UnixTimestamp);
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

            Author AuthorA = new Author { Id = "1", Name = "Bill", Email = "Bill@email.com", Cheeps = [] };
            Author AuthorB = new Author { Id = "2", Name = "Amy", Email = "Amy@email.com", Cheeps = [] };

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

    #endregion

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
        Assert.Equal(1, chirpContext.Cheeps.Count());
        var cheep = chirpContext.Cheeps.FirstOrDefault();
        Assert.NotNull(cheep);
        Assert.Equal(newCheep.Message, cheep.Message);
    }
}