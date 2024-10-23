using System.Net;
using System.Security.Principal;
using Chirp.Razor.DataModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleDB;
using SimpleDB.Model;

namespace RepositoryTests;

public class CheepRepositoryUnitTest
{
    [Theory]
    [InlineData(1, 3, 3)] //from start
    [InlineData(2, 3, 3)] //random section from middle
    [InlineData(3, 3, 1)] //last page with fewer cheeps
    [InlineData(1, 7, 7)] //all cheeps
    [InlineData(1, 10, 7)] //more than all cheeps
    [InlineData(0, 3, 0)] //page 0
    [InlineData(-1, 3, 0)] //negative page
    [InlineData(1, -3, 0)] //negative pagesize
    [InlineData(1, 0, 0)] //pagesize 0
    [InlineData(4, 3, 0)] //pagesize * pageno > no of cheeps
    public async Task GetCheepsByPage_ReturnsCorrectNumberOfCheeps(int page, int pageSize, int expected)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
        .UseSqlite(connection)
        .Options;

        using (var context = new ChirpDBContext(options)) {
            context.Database.EnsureCreated();

            var author = new Author { Id = 1, Name = "Bill", Email = "Bill@email", Cheeps = []};

            for (int i = 0; i < 7; i++) {
                context.Cheeps.Add(new Cheep {Author = author, Message = $"test_{i}", TimeStamp = DateTime.Now});
            }

            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);

            var result = await CheepRepo.GetCheepsByPage(page, pageSize);
            var count = result.Count();

            Assert.Equal(expected, count);
        }
    }

    [Fact]
    public async Task GetCheepsByPage_ReturnsCorrectCheepsFromMiddlePages() {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
        .UseSqlite(connection)
        .Options;

        using (var context = new ChirpDBContext(options)) {
            context.Database.EnsureCreated();

            var author = new Author { Id = 1, Name = "Bill", Email = "Bill@email", Cheeps = []};
            List<Cheep> cheeps = [];

            for (int i = 0; i < 7; i++) {
                cheeps.Add(new Cheep {Author = author, Message = $"test_{i}", TimeStamp = DateTime.Now});
            }

            cheeps.Reverse();
            context.Cheeps.AddRange(cheeps);
            

            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);

            var result = await CheepRepo.GetCheepsByPage(2, 3);
            var resultList = result.ToList();

            for (int i = 0; i < 3; i++) {
                var expectedCheep = cheeps.ElementAt(i+3);
                var singleCheep = resultList.ElementAt(i);

                Assert.Equal(expectedCheep.Author.Name, singleCheep.Author);
                Assert.Equal(expectedCheep.Message, singleCheep.Message);
                Assert.Equal(((DateTimeOffset)expectedCheep.TimeStamp).ToUnixTimeSeconds(), singleCheep.UnixTimestamp);
            }
        }
    }

    [Theory]
    [InlineData(3,3)]
    public async Task GetCheepsByPage_ReturnsNoCheepsForEmptyRepository(int page, int pageSize)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
        .UseSqlite(connection)
        .Options;

        using (var context = new ChirpDBContext(options)) {
            context.Database.EnsureCreated();

            var CheepRepo = new CheepRepository(context);

            var result = await CheepRepo.GetCheepsByPage(page, pageSize);

            Assert.Empty(result.ToList());
        }
    }

    [Fact]
    public async Task GetCheepsByPage_ReturnsCorrectSingleCheep()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
        .UseSqlite(connection)
        .Options;

        using (var context = new ChirpDBContext(options)) {
            context.Database.EnsureCreated();

            var author = new Author { Id = 1, Name = "Bill", Email = "Bill@email.com", Cheeps = []};
            var ts = new DateTime(2000, 01, 01);
            context.Cheeps.Add(new Cheep { Id = 1, Author = author, Message = "test", TimeStamp = ts});

            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);

            var result = await CheepRepo.GetCheepsByPage(1, 1);
            var resultList = result.ToList();

            var expected = new CheepDTO("Bill", "test", ((DateTimeOffset)ts).ToUnixTimeSeconds());

            Assert.Single(resultList);

            var singleCheep = resultList.ElementAt(0);

            Assert.Equal(expected.Author, singleCheep.Author);
            Assert.Equal(expected.Message, singleCheep.Message);
            Assert.Equal(expected.UnixTimestamp, singleCheep.UnixTimestamp);
        }
    }    
}   