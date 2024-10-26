using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RepositoryTests;

public class CheepRepositoryUnitTest
{
    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsCorrectCheepWhenMultipleAuthorsInDB() {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
        .UseSqlite(connection)
        .Options;

        using (var context = new ChirpDBContext(options)) {
            context.Database.EnsureCreated();

            List<Author> authors = []; List<Cheep> cheeps = [];

            for (int i = 0; i < 5; i++) {
                authors.Add(new Author { Name = $"name{i}", Email = $"{i}@mail.com", Cheeps = []});
                cheeps.Add(new Cheep { Author = authors.ElementAt(i), Message = $"test{i}", TimeStamp = DateTime.Now});
            }

            context.Authors.AddRange(authors);
            context.Cheeps.AddRange(cheeps);
            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);
            for (int i = 0; i < 5; i++) {
                var result = await CheepRepo.GetCheepsFromAuthorByPage(authors[i].Name, 1, 1);
                var resultDTO = result.ToList().ElementAt(0);

                Assert.Equal(cheeps[i].Author.Name, resultDTO.Author);
                Assert.Equal(cheeps[i].Message, resultDTO.Message);
                Assert.Equal(((DateTimeOffset)cheeps[i].TimeStamp).ToUnixTimeSeconds(), resultDTO.UnixTimestamp);
            }
        }
    }
    
    [Fact]
    public async Task GetCheepsFromAuthorByPage_ReturnsNoCheepsForNonexistentAuthor() {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
        .UseSqlite(connection)
        .Options;

        using (var context = new ChirpDBContext(options)) {
            context.Database.EnsureCreated();

            List<Author> authors = []; List<Cheep> cheeps = [];

            for (int i = 0; i < 5; i++) {
                authors.Add(new Author { Name = $"name{i}", Email = $"{i}@mail.com", Cheeps = []});
                cheeps.Add(new Cheep { Author = authors.ElementAt(i), Message = $"test{i}", TimeStamp = DateTime.Now});
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
    public async Task GetCheepsFromAuthorByPage_ReturnsAllCheepsFromAuthorForLargeNumberOfCheepsOnMultiplePages() {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
        .UseSqlite(connection)
        .Options;

        using (var context = new ChirpDBContext(options)) {
            context.Database.EnsureCreated();

            Author AuthorA = new Author {Name = "Bill", Email = "Bill@email.com", Cheeps = []};
            Author AuthorB = new Author {Name = "Amy", Email = "Amy@email.com", Cheeps = []};
            int authTotal = 0;
            var rand = new Random();

            for (int i = 0; i < 100; i++) {
                int r = rand.Next(2);
                if (r == 1) {
                    context.Cheeps.Add(new Cheep {Author = AuthorB, Message = "", TimeStamp = DateTime.Now});
                } else {
                    context.Cheeps.Add(new Cheep {Author = AuthorA, Message = "", TimeStamp = DateTime.Now});
                    authTotal++;
                }
            }

            context.SaveChanges();

            var CheepRepo = new CheepRepository(context);

            int pageNo = 1;
            int totalCount = 0;
            while (true) {
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
}