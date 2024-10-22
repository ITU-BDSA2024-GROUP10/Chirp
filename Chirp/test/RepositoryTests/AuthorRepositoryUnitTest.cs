using Chirp.Razor.DataModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RepositoryTests.Fixtures;
using SimpleDB;
using SimpleDB.Model;

namespace RepositoryTests;

public class AuthorRepositoryUnitTest : IClassFixture<InMemoryDBFixture<ChirpDBContext>>
{
    private readonly InMemoryDBFixture<ChirpDBContext> fixture;

    private readonly SqliteConnection Connection;
    
    public AuthorRepositoryUnitTest(InMemoryDBFixture<ChirpDBContext> fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task GetAuthorByName_NameExists_ReturnsAuthorDTO()
    {
        try
        {
            // Arrange
            await using (var context = fixture.GetContext())
            {
                IAuthorRepository repository = new AuthorRepository(context);
                var dto = new AuthorDTO("mr. test", "test@test.test");
                context.Authors.Add(new Author
                {
                    Name = dto.Name,
                    Email = dto.Email
                });
                context.SaveChanges();

                // Act
                var result = await repository.GetAuthorByName(dto.Name);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(dto.Name, result.Name);
                Assert.Equal(dto.Email, result.Email);
            }
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }
    
    [Fact]
    public async Task GetAuthorByName_NameDosentExists_ReturnsNull()
    {
        try
        {
            // Arrange
            await using (var context = fixture.GetContext())
            {
                IAuthorRepository repository = new AuthorRepository(context);
                var dto = new AuthorDTO("mr. test", "test@test.test");
                context.Authors.Add(new Author
                {
                    Name = dto.Name,
                    Email = dto.Email
                });
                context.SaveChanges();

                // Act
                var result = await repository.GetAuthorByName("not mr. test");

                // Assert
                Assert.Null(result);
            }
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }

    [Fact]
    public async Task GetAuthroByEmail_EmailExists_ReturnsAuthreDTO()
    {
        try
        {
            // Arrange
            await using (var context = fixture.GetContext())
            {
                IAuthorRepository repository = new AuthorRepository(context);
                var dto = new AuthorDTO("mr. test", "test@test.test");
                context.Authors.Add(new Author
                {
                    Name = dto.Name,
                    Email = dto.Email
                });
                context.SaveChanges();

                // Act
                var result = await repository.GetAuthorByEmail(dto.Email);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(dto.Name, result.Name);
                Assert.Equal(dto.Email, result.Email);
            }
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }
    
    [Fact]
    public async Task GetAuthroByEmail_EmailDosentExists_Returnsnull()
    {
        try
        {
            // Arrange
            await using (var context = fixture.GetContext())
            {
                IAuthorRepository repository = new AuthorRepository(context);
                var dto = new AuthorDTO("mr. test", "test@test.test");
                context.Authors.Add(new Author
                {
                    Name = dto.Name,
                    Email = dto.Email
                });
                context.SaveChanges();

                // Act
                var result = await repository.GetAuthorByEmail("not test@test.test");

                // Assert
                Assert.Null(result);
            }
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }
    
    [Fact]
    public async Task AddAuthor_AuthorDosentExistst_AutherGetsInserted()
    {
        try
        {
            // Arrange
            await using (var context = fixture.GetContext())
            {
                IAuthorRepository repository = new AuthorRepository(context);
                var dto = new AuthorDTO("mr. test", "test@test.test");

                // Act
                var result = repository.AddAuthor(dto);

                // Assert
                Assert.True(await result);
                Assert.Equal(1, context.Authors.Count());
                Assert.NotNull(context.Authors
                    .SingleOrDefault(a => a.Name == dto.Name &&
                                          a.Email == dto.Email &&
                                          a.Id == 1));
            }
        }
        finally
        {
            fixture.ResetDatabase();
        }
    }
}