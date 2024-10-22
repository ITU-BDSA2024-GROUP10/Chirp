using Chirp.Razor.DataModels;
using RepositoryTests.Fixtures;
using SimpleDB;
using SimpleDB.Model;

namespace RepositoryTests;

public class CheepRepositoryUnitTest : IClassFixture<InMemoryDBFixture<ChirpDBContext>>
{
    private readonly InMemoryDBFixture<ChirpDBContext> fixture;

    public CheepRepositoryUnitTest(InMemoryDBFixture<ChirpDBContext> fixture)
    {
        this.fixture = fixture;
    }

    [Theory]
    [InlineData(1, 10, 13, 10)]
    [InlineData(2, 10, 13, 3)]
    [InlineData(3, 10, 13, 0)]
    public async Task GetCheepsByPage_ReturnsCheeps(int page, int pageSize,
        int totalAmountOfCheeps, int expectedCheeps)
    {
        try
        {
            // Arrange
            var context = fixture.GetContext();
            ICheepRepository cheepRepository = new CheepRepository(context);

            var author = new Author
            {
                Name = "TestAuthor",
                Email = "Test@test.test"
            };

            var cheeps = new List<Cheep>();

            context.Authors.Add(author);
            for (int i = 0; i < totalAmountOfCheeps; i++)
            {
                cheeps.Add(new Cheep
                {
                    Author = author,
                    Message = "TestMessage",
                    TimeStamp = DateTime.Now
                });
            }
            cheeps.Reverse();
            context.Cheeps.AddRange(cheeps);
            cheeps.Reverse();
            context.SaveChanges();

            //act
            var result = await cheepRepository.GetCheepsByPage(page, pageSize);

            //assert
            Assert.Equal(expectedCheeps, result.Count());
            for (int i = 0; i < result.Count(); i++)
            {
                var resultDTO = result.ElementAt(i);
                var dto = new CheepDTO(
                    author.Name,
                    cheeps[i].Message,
                    cheeps[i].TimeStamp
                );
                
                Assert.Equal(dto.Author, resultDTO.Author);
                Assert.Equal(dto.Message, resultDTO.Message);
                Assert.Equal(dto.UnixTimestamp, resultDTO.UnixTimestamp);
            }
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }

    [Theory]
    [InlineData(1, 10, 13, 10)]
    [InlineData(2, 10, 13, 3)]
    [InlineData(3, 10, 13, 0)]
    public async Task GetCheepsFromAuthorByPage_ReturnsCheeps(int page, int pageSize,
        int totalAmountOfCheeps, int expectedCheeps)
    {
        try
        {
            // Arrange
            var context = fixture.GetContext();
            ICheepRepository cheepRepository = new CheepRepository(context);

            var author = new Author
            {
                Name = "TestAuthor",
                Email = "Test@test.test"
            };

            var fakeAuthor = new Author
            {
                Name = "FakeAuthor",
                Email = "fake@test.test"
            };

            context.Authors.Add(author);
            context.Authors.Add(fakeAuthor);
            
            var cheeps = new List<Cheep>();
            
            for (int i = 0; i < totalAmountOfCheeps; i++)
            {
                cheeps.Add(new Cheep
                {
                    Author = author,
                    Message = "TestMessage",
                    TimeStamp = DateTime.Now
                });
            }

            for (int i = 0; i < totalAmountOfCheeps; i++)
            {
                context.Cheeps.Add(new Cheep
                {
                    Author = fakeAuthor,
                    Message = "TestMessage",
                    TimeStamp = DateTime.Now
                });
            }

            cheeps.Reverse();
            context.Cheeps.AddRange(cheeps);
            cheeps.Reverse();
            context.SaveChanges();

            //act
            var result = await cheepRepository.GetCheepsFromAuthorByPage(author.Name, page, pageSize);

            //assert
            Assert.Equal(expectedCheeps, result.Count());
            for (int i = 0; i < result.Count(); i++)
            {
                var resultDTO = result.ElementAt(i);
                var dto = new CheepDTO(
                    author.Name,
                    cheeps[i].Message,
                    cheeps[i].TimeStamp
                );
                
                Assert.Equal(dto.Author, resultDTO.Author);
                Assert.Equal(dto.Message, resultDTO.Message);
                Assert.Equal(dto.UnixTimestamp, resultDTO.UnixTimestamp);
            }
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }

    [Fact]
    public async Task CreateCheep_ReturnsTrue()
    {
        try
        {
            // Arrange
            var context = fixture.GetContext();
            ICheepRepository cheepRepository = new CheepRepository(context);

            var author = new Author
            {
                Name = "TestAuthor",
                Email = "test@test.test"
            };

            var dto = new CheepDTO(
                author.Name,
                "TestMessage",
                DateTimeOffset.Now.ToUnixTimeSeconds()
            );

            context.Authors.Add(author);
            context.SaveChanges();

            //act
            var result = await cheepRepository.CreateCheep(dto);

            //assert
            Assert.True(result);
            Assert.Equal(1, context.Cheeps.Count());
            Assert.NotNull(context.Cheeps.FirstOrDefault(
                c => c.Author.Name == author.Name &&
                     c.Message == dto.Message &&
                     c.TimeStamp == DateTimeOffset.FromUnixTimeSeconds(dto.UnixTimestamp).DateTime
            ));
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }
}