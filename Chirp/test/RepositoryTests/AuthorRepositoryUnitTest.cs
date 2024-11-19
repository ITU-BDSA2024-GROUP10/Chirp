using Chirp.Core;
using Chirp.Core.CustomException;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using RepositoryTests.Utils;

namespace RepositoryTests;

public class AuthorRepositoryUnitTest(InMemoryDBFixture<ChirpDBContext> fixture)
    : IClassFixture<InMemoryDBFixture<ChirpDBContext>> 
{

    [Fact]
    public async Task GetAuthorByName_NameCantBeFound_ThrowsException()
    {
        //arrange
        var chirpContext = fixture.GetContext();
        var author = new Author { UserName = "John Doe", Email = "JohnDoe@gmail.com" };

        chirpContext.Authors.Add(author);
        await chirpContext.SaveChangesAsync();

        IAuthorRepository authorRepo = new AuthorRepository(chirpContext);

        var newAuthor = new Author { UserName = "Abra Cabrera", Email = "AbraCabrera@gmail.com" };

        //act & assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => authorRepo.GetAuthorByName(newAuthor.UserName));
    }

    [Fact]
    public async void GetAuthorByName_NameIsHelge_ReturnsAuthorDTOOfHelge()
    {
        //Arrange an arbitrary author with name 'Helge' and create arbitrary database to put up
        var chirpContext = fixture.GetContext();
        var author = new Author { UserName = "Helge", Email = "Helge@gmail.com" };
        author.NormalizedUserName = author.UserName.ToUpper();
        
        chirpContext.Authors.Add(author);
        await chirpContext.SaveChangesAsync();

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);

        //Act a scenario where the repository can get an author by the name of 'Helge'
        var result = await authorRepository.GetAuthorByName(author.UserName);

        //Assert the value of the arbitrary author that was arranged
        Assert.NotNull(result);
        Assert.Equal("Helge", result.Name);
        Assert.Equal("Helge@gmail.com", result.Email);
    }

    [Fact]
    public async void AddAuthor_NameIsNullKeyword_ReturnFalse()
    {
        //Arrange
        var chirpContext = fixture.GetContext();
        var author = new AuthorDTO(null!, "null@gmail.com");

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
        var chirpContext = fixture.GetContext();
        var author = new AuthorDTO("John Doe", "JohnDoe@gmail.com");

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);

        //Act
        var result = await authorRepository.AddAuthor(author);
        var checkSuccession = chirpContext.Authors.Where(a => a.Email == author.Email).Select(a => new Author
        {
            Id = a.Id,
            UserName = a.UserName,
            Email = a.Email
        }).FirstOrDefault();

        //Assert
        Assert.True(result);
        Assert.NotNull(checkSuccession);
        Assert.Equal(author.Email, checkSuccession.Email);
        Assert.Equal(author.Name, checkSuccession.UserName);
    }
}