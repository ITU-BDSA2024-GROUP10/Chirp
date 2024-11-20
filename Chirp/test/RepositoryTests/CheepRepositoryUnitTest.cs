using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RepositoryTests.Utils;

namespace RepositoryTests;

public class CheepRepositoryUnitTest(InMemoryDBFixture<ChirpDBContext> _fixture)
    : IClassFixture<InMemoryDBFixture<ChirpDBContext>>
{

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
        var chirpContext = _fixture.GetContext();
        var author = new Author { Id = "1", UserName = "Bill", Email = "Bill@email", Cheeps = [] };
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
        var chirpContext = _fixture.GetContext();
        var author = new Author { UserName = "mr. test", Email = "test@test.test" };
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
        var chirpContext = _fixture.GetContext();
        var author = new Author { Id = "1", UserName = "Bill", Email = "Bill@email", Cheeps = [] };
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

            Assert.Equal(expectedCheep.Author.UserName, singleCheep.Author);
            Assert.Equal(expectedCheep.Message, singleCheep.Message);
            Assert.Equal(((DateTimeOffset)expectedCheep.TimeStamp).ToUnixTimeSeconds(), singleCheep.UnixTimestamp);
        }
    }

    [Theory]
    [InlineData(3, 3)]
    public async Task GetCheepsByPage_ReturnsNoCheepsForEmptyRepository(int page, int pageSize)
    {
        //arrange
        var chirpContext = _fixture.GetContext();
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
        var chirpContext = _fixture.GetContext();
        var author = new Author { Id = "1", UserName = "Bill", Email = "Bill@email.com", Cheeps = [] };
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
        var chirpContext = _fixture.GetContext();
        List<Author> authors = [];
        List<Cheep> cheeps = [];

        for (int i = 0; i < 5; i++)
        {
            var author = new Author { UserName = $"name{i}", Email = $"{i}@mail.com", Cheeps = [] };
            author.NormalizedUserName = author.UserName.ToUpper();
            authors.Add(author);
            var cheep = new Cheep { Author = authors.ElementAt(i), Message = $"test{i}", TimeStamp = DateTime.Now };
            cheeps.Add(cheep);
        }

        chirpContext.Authors.AddRange(authors);
        chirpContext.Cheeps.AddRange(cheeps);
        await chirpContext.SaveChangesAsync();

        var cheepRepo = new CheepRepository(chirpContext);
        for (int i = 0; i < 5; i++)
        {
            //act
            var result = await cheepRepo.GetCheepsFromAuthorByPage(authors[i].UserName!, 1, 1);
            var resultDTO = result.ToList().ElementAt(0);

            //arrange
            Assert.Equal(cheeps[i].Author.UserName, resultDTO.Author);
            Assert.Equal(cheeps[i].Message, resultDTO.Message);
            Assert.Equal(((DateTimeOffset)cheeps[i].TimeStamp).ToUnixTimeSeconds(), resultDTO.UnixTimestamp);
        }
    }

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsNoCheepsForNonexistentAuthor()
    {
        //arrange
        var chirpContext = _fixture.GetContext();
        List<Author> authors = [];
        List<Cheep> cheeps = [];

        for (int i = 0; i < 5; i++)
        {
            var author = new Author { UserName = $"name{i}", Email = $"{i}@mail.com" };
            authors.Add(author);
            var cheep = new Cheep { Author = authors.ElementAt(i), Message = $"test{i}", TimeStamp = DateTime.Now };
            cheeps.Add(cheep);
        }

        chirpContext.Authors.AddRange(authors);
        chirpContext.Cheeps.AddRange(cheeps);
        await chirpContext.SaveChangesAsync();

        var cheepRepo = new CheepRepository(chirpContext);

        //act
        var result = await cheepRepo.GetCheepsFromAuthorByPage("Bill", 1, 5);

        //assert
        Assert.Empty(result.ToList());
    }

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsAllCheepsFromAuthorForLargeNumberOfCheepsOnMultiplePages()
    {
        //arrange
        var chirpContext = _fixture.GetContext();
        var authorA = new Author { Id = "1", UserName = "Bill", Email = "Bill@email.com", Cheeps = [] };
        var authorB = new Author { Id = "2", UserName = "Amy", Email = "Amy@email.com", Cheeps = [] };
        authorA.NormalizedUserName = authorA.UserName.ToUpper();
        authorB.NormalizedUserName = authorB.UserName.ToUpper();
        
        var authTotal = 0;
        var rand = new Random();

        for (int i = 0; i < 100; i++)
        {
            int r = rand.Next(2);
            if (r == 1)
            {
                var cheep = new Cheep { Author = authorB, Message = "", TimeStamp = DateTime.Now };
                chirpContext.Cheeps.Add(cheep);
            }
            else
            {
                var cheep = new Cheep { Author = authorA, Message = "", TimeStamp = DateTime.Now };
                chirpContext.Cheeps.Add(cheep);
                authTotal++;
            }
        }

        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);

        //act
        int pageNo = 1;
        int totalCount = 0;
        while (true)
        {
            var result = await cheepRepo.GetCheepsFromAuthorByPage("Bill", pageNo, 20);
            var count = result.ToList().Count;
            if (count == 0)
                break;
            totalCount += count;
            pageNo++;
        }

        //assert
        Assert.Equal(authTotal, totalCount);
    }

    #endregion

    [Fact]
    public async Task CreateCheep()
    {
        //arrange
        var chirpContext = _fixture.GetContext();
        var author = new Author { UserName = "John Doe", Email = "JohnDoe@gmail.com" };
        chirpContext.Authors.Add(author);
        await chirpContext.SaveChangesAsync();

        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var newCheep = new CheepDTO("John Doe", "message", unixTimestamp);

        var cheepRepo = new CheepRepository(chirpContext);

        //act
        var result = await cheepRepo.CreateCheep(newCheep);

        //Assert
        Assert.True(result);
        Assert.Equal(1, chirpContext.Cheeps.Count());
        var cheep = chirpContext.Cheeps.FirstOrDefault();
        Assert.NotNull(cheep);
        Assert.Equal(newCheep.Message, cheep.Message);
    }

    [Fact]
    public async Task GetCheepsFromAuthorsByPage_ReturnsAllCheepsFromAuthorsFromMultiplePages()
    {
        //Arrange
        var chirpContext = _fixture.GetContext();
        var author1 = new Author { UserName = "Mr. Test", Email = "test@email.com" };
        var author2 = new Author { UserName = "Mr. Test2", Email = "test2@email.com" };
        
        author1.NormalizedUserName = author1.UserName.ToUpper();
        author2.NormalizedUserName = author2.UserName.ToUpper();

        List<string> authors = new();
        
        
        authors.Add(author1.UserName);
        authors.Add(author2.UserName);
        
        var authTotal = 0;
        var rand = new Random();
        
        for (var i = 0; i < 100; i++)
        {
            int r = rand.Next(2);
            if (r == 1)
            {
                var cheep = new Cheep { Author = author1, Message = "", TimeStamp = DateTime.Now };
                chirpContext.Cheeps.Add(cheep);
                authTotal++;
            }
            else
            {
                var cheep = new Cheep { Author = author2, Message = "", TimeStamp = DateTime.Now };
                chirpContext.Cheeps.Add(cheep);
                authTotal++;
            }
        }
        
        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);
        
        int pageNo = 1;
        int totalCount = 0;
        
        //Act
        while (true)
        {
            var result = await cheepRepo.GetCheepsFromAuthorsByPage(authors, pageNo, 20);
            var count = result.ToList().Count;
            if (count == 0)
                break;
            totalCount += count;
            pageNo++;
        }
        
        //Assert
        Assert.Equal(authTotal, totalCount);
    }

    [Fact]
    public async Task GetCheepsFromAuthorsByPage_ReturnsNoCheepsFromAuthorWithNoCheeps()
    {
        //Arrange
        var chirpContext = _fixture.GetContext();
        var author1 = new Author { UserName = "Mr. Test", Email = "test@email.com"};
        var author2 = new Author { UserName = "Mr. Test2", Email = "test2@email.com"};

        author1.NormalizedUserName = author1.UserName.ToUpper();
        author2.NormalizedUserName = author1.UserName.ToUpper();

        List<string> authors = new();
        List<Cheep> cheeps = [];
        authors.Add(author1.UserName);
        authors.Add(author2.UserName);

        chirpContext.Cheeps.AddRange(cheeps);
        
        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);
        
        //Act
        var result = await cheepRepo.GetCheepsFromAuthorsByPage(authors, 1, 20);
        
        //Assert
        Assert.Empty(result.ToList());
    }

    [Fact]
    public async Task GetCheepsFromAuthorsByPage_ReturnsInChronologicalOrder()
    {
        //Arrange
        var chirpContext = _fixture.GetContext();
        var author1 = new Author { UserName = "Mr. Test", Email = "test@email.com" };

        author1.NormalizedUserName = author1.UserName.ToUpper();

        List<string> authors = new();
        authors.Add(author1.UserName);

        for (var i = 0; i < 5; i++)
        {
            var cheep = new Cheep { Author = author1, Message = $"{i}", TimeStamp = DateTimeOffset.FromUnixTimeSeconds(i * 60).DateTime };
            chirpContext.Cheeps.Add(cheep);
        }
        
        await chirpContext.SaveChangesAsync();
        var cheepRepo = new CheepRepository(chirpContext);
        
        //Act
        var result = await cheepRepo.GetCheepsFromAuthorsByPage(authors, 1, 20);
        var resultArray = result.ToArray();

        //Assert
        Assert.True(resultArray[0].UnixTimestamp >= resultArray[1].UnixTimestamp);
        
        for (var i = 1; i < resultArray.ToList().Count; i++)
        {
            Assert.True(resultArray[i-1].UnixTimestamp >= resultArray[i].UnixTimestamp);
        }
    }
}