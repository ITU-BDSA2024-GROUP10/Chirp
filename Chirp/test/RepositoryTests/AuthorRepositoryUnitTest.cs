using Chirp.Razor.DataModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using SimpleDB;
using SimpleDB.Model;

namespace RepositoryTests;

public class AuthorRepositoryUnitTest : IDisposable
{
    private readonly SqliteConnection _connection;

    public AuthorRepositoryUnitTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
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
    public async void GetAuthorByName_NameCantBeFound_ReturnNull()
    {
        //arrange
        var chirpContext = GetContext();
        var author = new Author {Name = "John Doe", Email = "JohnDoe@gmail.com"};

        chirpContext.Authors.Add(author);
        chirpContext.SaveChanges();

        IAuthorRepository authorRepo = new AuthorRepository(chirpContext);

        var newAuthor = new Author { Name = "Abra Cabrera", Email = "AbraCabrera@gmail.com" };

        //Act
        var result = await authorRepo.GetAuthorByName(newAuthor.Name);
        
        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async void GetAuthorByName_NameIsHelge_ReturnsAuthorDTOOfHelge()
    {
        //Arrange an arbitrary author with name 'Helge' and create arbitrary database to put up
        var chirpContext = GetContext();
        var author = new Author { Name = "Helge", Email = "Helge@gmail.com" };

        chirpContext.Authors.Add(author);
        chirpContext.SaveChanges();

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);

        //Act a scenario where the repository can get an author by the name of 'Helge'
        var result = await authorRepository.GetAuthorByName(author.Name);

        //Assert the value of the arbitrary author that was arranged
        Assert.Equal("Helge", result.Name);
        Assert.Equal("Helge@gmail.com", result.Email);
    }
    
    [Fact]
    public async void AddAuthor_NameIsNullKeyword_ReturnFalse()
    {
        //Arrange
        var chirpContext = GetContext();
        var author = new AuthorDTO("null", "null@gmail.com");

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);
        
        //Act
        var result = await authorRepository.AddAuthor(author);
        
        //Assert
        Assert.False(result); //Since the name isn't valid the operation was unsuccessful
    }

    [Fact]
    public async void AddAuthor_NameIsJohn_Doe_ReturnTrue()
    {
        //Arrange
        var chirpContext = GetContext();
        var author = new AuthorDTO("John Doe", "JohnDoe@gmail.com");

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);
        
        //Act
        var result = await authorRepository.AddAuthor(author);
        var checkSuccession = chirpContext.Authors.Where(a => a.Email == author.Email).Select(a => new Author
        {
            Id = a.Id,
            Name = a.Name,
            Email = a.Email
        }).FirstOrDefault();
        
        //Assert
        Assert.True(result);
        Assert.NotNull(checkSuccession);
        Assert.Equal(author.Email, checkSuccession.Email);
        Assert.Equal(author.Name, checkSuccession.Name);
        Assert.Equal(1, checkSuccession.Id);
    }

    [Fact]
    public async void GetAuthorByEmail_EmailIsAnITUMail_ReturnAuthorDTOOfMail()
    {
        //Arrange
        var chirpContext = GetContext();
        var author = new Author { Name = "John Doe", Email = "jodoe@itu.dk" };

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);
        
        //Act
        var result = await authorRepository.GetAuthorByEmail(author.Email);
        
        //Assert
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("jodoe@itu.dk", result.Email);
    }

    public void Dispose()
    {
        _connection.Close();
    }

    [Fact]
    public async void GetAuthorByEmail_EmailIsNotThere_ReturnNull()
    {
        //Arrange
        var chirpContext = GetContext();
        var author = new AuthorDTO("John Doe", "jodoe@itu.dk");
        var author2 = new Author { Name = "Abra Cabrera", Email = "AbraCabrera@gmail.com" };

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);
        await authorRepository.AddAuthor(author);
        
        //Act
        var result = await authorRepository.GetAuthorByEmail(author2.Email);
        
        //Assert
        Assert.Null(result);

    }
}