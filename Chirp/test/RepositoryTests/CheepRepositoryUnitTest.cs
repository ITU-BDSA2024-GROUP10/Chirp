using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RepositoryTests.Utils;
using TestUtilities;

namespace RepositoryTests;

public class CheepRepositoryUnitTest
    : IClassFixture<InMemoryDBFixture<ChirpDBContext>>
{
    protected ChirpDBContext Context { get; }
    protected ICheepRepository CheepRepository { get; }
    
    public CheepRepositoryUnitTest(InMemoryDBFixture<ChirpDBContext> fixture)
    {
        fixture.ResetDatabase();
        Context = fixture.GetContext();
        CheepRepository = new CheepRepository(fixture.GetContext());
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
        var author = TestUtils.CreateTestAuthor("Mr. test");
        for (int i = 0; i < 7; i++)
        {
            var cheep = new Cheep { Author = author, Message = $"test_{i}", TimeStamp = DateTime.Now };
            Context.Cheeps.Add(cheep);
        }

        await Context.SaveChangesAsync();

        //act
        var result = await CheepRepository.GetCheepsByPage(page, pageSize);

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.Count());
    }

    [Fact]
    public async Task GetCheepsByPage_NegativePageSize_ReturnsNull()
    {
        //arrange
        var author = TestUtils.CreateTestAuthor("Mr. test");
        var cheep = new Cheep { Author = author, Message = "test", TimeStamp = DateTime.Now };
       
        Context.Cheeps.Add(cheep);
        await Context.SaveChangesAsync();

        //act
        var result = await CheepRepository.GetCheepsByPage(1, -1);

        //assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCheepsByPage_ReturnsCorrectCheepsFromMiddlePages()
    {
        //arrange
        var author = TestUtils.CreateTestAuthor("Mr. test");
        List<Cheep> cheeps = [];
        for (int i = 0; i < 7; i++)
        {
            var cheep = new Cheep { Author = author, Message = $"test_{i}", TimeStamp = DateTime.Now };
            cheeps.Add(cheep);
        }

        cheeps.Reverse();
        Context.Cheeps.AddRange(cheeps);
        await Context.SaveChangesAsync();

        //act
        var result = await CheepRepository.GetCheepsByPage(2, 3);

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
    [InlineData(1, 3)]
    public async Task GetCheepsByPage_ReturnsNoCheepsForEmptyRepository(int page, int pageSize)
    {
        //act
        var result = await CheepRepository.GetCheepsByPage(page, pageSize);

        //assert
        Assert.NotNull(result);
        Assert.Empty(result.ToList());
    }

    [Fact]
    public async Task GetCheepsByPage_ReturnsCorrectSingleCheep()
    {
        //arrange
        var author = TestUtils.CreateTestAuthor("Mr. test");
        var timeStamp = new DateTime(2000, 01, 01);
        var cheep = new Cheep { Author = author, Message = "test", TimeStamp = timeStamp };
        
        Context.Cheeps.Add(cheep);
        await Context.SaveChangesAsync();

        //act
        var result = await CheepRepository.GetCheepsByPage(1, 1);

        //assert
        Assert.NotNull(result);

        var resultList = result.ToList();
        var expected = new CheepDTO(null, author.UserName!, cheep.Message, ((DateTimeOffset)timeStamp).ToUnixTimeSeconds());
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
        List<Author> authors = [];
        List<Cheep> cheeps = [];

        for (int i = 0; i < 5; i++)
        {
            var author = TestUtils.CreateTestAuthor($"name{i}");
            authors.Add(author);
            
            var cheep = new Cheep { Author = author, Message = $"test{i}", TimeStamp = DateTime.Now };
            cheeps.Add(cheep);
        }

        Context.Authors.AddRange(authors);
        Context.Cheeps.AddRange(cheeps);
        await Context.SaveChangesAsync();
        
        for (int i = 0; i < 5; i++)
        {
            //act
            var result = await CheepRepository.GetCheepsFromAuthorByPage(authors[i].UserName!, 1, 1);
            var resultDTO = result.ToList().ElementAt(0);

            //arrange
            Assert.Equal(cheeps[i].Author.UserName, resultDTO.Author);
            Assert.Equal(cheeps[i].Message, resultDTO.Message);
            Assert.Equal(((DateTimeOffset)cheeps[i].TimeStamp).ToUnixTimeSeconds(), resultDTO.UnixTimestamp);
        }
    }

    [Fact]
    public async Task GetCheepsFromAuthor_ReturnsAllCheeps()
    {
        var author = TestUtils.CreateTestAuthor("Mr. test");
        List<Cheep> cheeps = [];
        for (int i = 0; i < 5; i++)
        {
            cheeps.Add(new Cheep{ Author = author, Message = "test" + i, TimeStamp = DateTime.Now });
        }

        Context.Authors.Add(author);
        Context.Cheeps.AddRange(cheeps);
        await Context.SaveChangesAsync();
        
        var result = (await CheepRepository.GetCheepsFromAuthor(author.UserName!)).ToList();
        cheeps.Reverse();
        Assert.Equal(5, result.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(cheeps.ElementAt(i).Message, result.ElementAt(i).Message);
        }
        

    }

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsNoCheepsForNonexistentAuthor()
    {
        //arrange
        List<Author> authors = [];
        List<Cheep> cheeps = [];

        for (int i = 0; i < 5; i++)
        {
            var author = TestUtils.CreateTestAuthor($"name{i}");
            authors.Add(author);
            
            var cheep = new Cheep { Author = author, Message = $"test{i}", TimeStamp = DateTime.Now };
            cheeps.Add(cheep);
        }

        Context.Authors.AddRange(authors);
        Context.Cheeps.AddRange(cheeps);
        await Context.SaveChangesAsync();

        //act
        var result = await CheepRepository.GetCheepsFromAuthorByPage("Bill", 1, 5);

        //assert
        Assert.Empty(result.ToList());
    }

    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsAllCheepsFromAuthorForLargeNumberOfCheepsOnMultiplePages()
    {
        //arrange
        var authorA = TestUtils.CreateTestAuthor("Mr. test");
        var authorB = TestUtils.CreateTestAuthor("Mr. fake");
        
        var authTotal = 0;

        for (int i = 0; i < 100; i++)
        {
            if (i % 2 == 0)
            {
                var cheep = new Cheep { Author = authorB, Message = "", TimeStamp = DateTime.Now };
                Context.Cheeps.Add(cheep);
            }
            else
            {
                var cheep = new Cheep { Author = authorA, Message = "", TimeStamp = DateTime.Now };
                Context.Cheeps.Add(cheep);
                authTotal++;
            }
        }

        await Context.SaveChangesAsync();

        //act
        int pageNo = 1;
        int totalCount = 0;
        while (true)
        {
            var result = await CheepRepository.GetCheepsFromAuthorByPage(authorA.UserName!, pageNo, 20);
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
        var author = TestUtils.CreateTestAuthor("Mr. test");
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();

        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var newCheep = new CheepDTO(null, author.UserName!, "message", unixTimestamp);

        //act
        var result = await CheepRepository.CreateCheep(newCheep);

        //Assert
        Assert.True(result);
        Assert.Equal(1, Context.Cheeps.Count());
        var cheep = Context.Cheeps.FirstOrDefault();
        Assert.NotNull(cheep);
        Assert.Equal(newCheep.Message, cheep.Message);
    }

    [Fact]
    public async Task GetCheepsFromAuthorsByPage_ReturnsAllCheepsFromAuthorsFromMultiplePages()
    {
        //Arrange
        var author1 = TestUtils.CreateTestAuthor("Mr. Test");
        var author2 = TestUtils.CreateTestAuthor("Mr. Test2");

        List<string> authors = new();
        
        
        authors.Add(author1.UserName!);
        authors.Add(author2.UserName!);
        
        for (var i = 0; i < 100; i++)
        {
            if (i % 2 == 0)
            {
                var cheep = new Cheep { Author = author1, Message = "", TimeStamp = DateTime.Now };
                Context.Cheeps.Add(cheep);
            }
            else
            {
                var cheep = new Cheep { Author = author2, Message = "", TimeStamp = DateTime.Now };
                Context.Cheeps.Add(cheep);
            }
        }
        
        await Context.SaveChangesAsync();
        
        int pageNo = 1;
        int totalCount = 0;
        
        //Act
        while (true)
        {
            var result = await CheepRepository.GetCheepsFromAuthorsByPage(authors, pageNo, 20);
            var count = result.ToList().Count;
            if (count == 0)
                break;
            totalCount += count;
            pageNo++;
        }
        
        //Assert
        Assert.Equal(100, totalCount);
    }

    [Fact]
    public async Task GetCheepsFromAuthorsByPage_ReturnsNoCheepsFromAuthorWithNoCheeps()
    {
        //Arrange
        var author1 = TestUtils.CreateTestAuthor("Mr. Test");
        var author2 = TestUtils.CreateTestAuthor("Mr. Test2");

        List<string> authors = new();
        List<Cheep> cheeps = [];
        authors.Add(author1.UserName!);
        authors.Add(author2.UserName!);

        Context.Cheeps.AddRange(cheeps);
        for (int i = 0; i < 7; i++)
        {
            var cheep1 = new Cheep { Author = author1, Message = $"test_{i}", TimeStamp = DateTime.Now };
            var cheep2 = new Cheep { Author = author2, Message = $"test_{i}", TimeStamp = DateTime.Now };
            cheeps.Add(cheep1);
            cheeps.Add(cheep2);
        }
        
        await Context.SaveChangesAsync();
        
        //Act
        var result = await CheepRepository.GetCheepsFromAuthorsByPage(authors, 1, 20);
        
        //Assert
        Assert.Empty(result.ToList());
    }

    [Fact]
    public async Task GetCheepsFromAuthorsByPage_ReturnsInChronologicalOrder()
    {
        //Arrange
        var author1 = TestUtils.CreateTestAuthor("Mr. Test");

        List<string> authors = new();
        authors.Add(author1.UserName!);

        for (var i = 0; i < 5; i++)
        {
            var cheep = new Cheep { Author = author1, Message = $"{i}", TimeStamp = DateTimeOffset.FromUnixTimeSeconds(i * 60).DateTime };
            Context.Cheeps.Add(cheep);
        }
        
        await Context.SaveChangesAsync();
        
        //Act
        var result = await CheepRepository.GetCheepsFromAuthorsByPage(authors, 1, 20);
        var resultArray = result.ToArray();

        //Assert
        Assert.True(resultArray[0].UnixTimestamp >= resultArray[1].UnixTimestamp);
        
        for (var i = 1; i < resultArray.ToList().Count; i++)
        {
            Assert.True(resultArray[i-1].UnixTimestamp >= resultArray[i].UnixTimestamp);
        }
    }

    [Fact]
    public async Task GetCheepById_ReturnsCorrectCheep()
    {
        // Arrange TEST nr 100 lez gooo
        var author = TestUtils.CreateTestAuthor("mr. test");
        var cheep = new Cheep
        {
            Id = 0,
            Message = "test",
            TimeStamp = DateTime.Now,
            Author = author
        };
        
        Context.Authors.Add(author);
        Context.Cheeps.Add(cheep);
        await Context.SaveChangesAsync();
        
        var result = await CheepRepository.GetCheepById(cheep.Id);
        Assert.NotNull(result);
        Assert.Equal(cheep.Id, result.Id);
        Assert.Equal(cheep.Message, result.Message);
    }
}