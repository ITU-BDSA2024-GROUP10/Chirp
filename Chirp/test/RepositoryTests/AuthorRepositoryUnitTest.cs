using Chirp.Core;
using Chirp.Core.CustomException;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using RepositoryTests.Utils;
using TestUtilities;

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
    public async Task GetAuthorByName_AuthorExist_ReturnsAuthorDTOOfAuthor()
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
    public async Task AddAuthor_NameIsNullKeyword_ReturnFalse()
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
    public async Task AddAuthor_UserDosentExistYet_ReturnTrue()
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

    #region GetAuthorFollows tests

    [Fact]
    public async Task GetAuthorFollows_UserDoesNotExist_ThrowsException()
    {
        var authorRepo = new AuthorRepository(fixture.GetContext());
        await Assert.ThrowsAsync<UserDoesNotExist>(() => authorRepo.GetAuthorFollows("mr. test"));
    }

    [Fact]
    public async Task GetAuthorFollows_UserExists_ReturnsListOfAuthors()
    {
        #region Arrange

        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);

        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        var notFollowing = TestUtils.CreateTestAuthor("mr. not follow");
        
        author.Following.Add(following);

        context.Authors.Add(notFollowing);
        context.Authors.Add(following);
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        #endregion

        //Act
        var result = await authorRepo.GetAuthorFollows(author.UserName!);
        
        //Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal(following.UserName, result[0].Name);
    }

    #endregion

    #region Follow tests

    [Fact]
    public async Task Follow_UserToFollowDoesNotExist_ThrowsException()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => authorRepo.Follow(author.UserName!, "mr. follow"));
    }

    [Fact]
    public async Task Follow_UserDoesNotExist_ThrowsException()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. follow");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => authorRepo.Follow("mr. test", author.UserName!));
    }

    [Fact]
    public async Task Follow_UserToFollowIsItSelf_ThrowsException()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => authorRepo.Follow(author.UserName!, author.UserName!));
    }

    [Fact]
    public async Task Follow_UserToFollowAlreadyFollowed_ReturnsFalse()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        author.Following.Add(following);
        
        context.Authors.Add(following);
        context.Authors.Add(author);
        
        await context.SaveChangesAsync();
        
        //Act
        var result = await authorRepo.Follow(author.UserName!, following.UserName!);
        
        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Follow_UserToFollowIsNotFollowed_ReturnsTrue()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        
        context.Authors.Add(following);
        context.Authors.Add(author);
        
        await context.SaveChangesAsync();
        
        //Act
        var result = await authorRepo.Follow(author.UserName!, following.UserName!);
        var authorsFollowing = context.Authors
            .Where(a => a.NormalizedUserName == author.NormalizedUserName)
            .SelectMany(a => a.Following).ToList();
        
        //Assert
        Assert.True(result);
        Assert.NotEmpty(authorsFollowing);
        Assert.Single(authorsFollowing);
        Assert.Equal(following.UserName, authorsFollowing[0].UserName);
        
    }

    #endregion

    #region UnFollow tests

    [Fact]
    public async Task UnFollow_UserToUnfollowDoesNotExist_ThrowsException()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => authorRepo.UnFollow(author.UserName!, "mr. follow"));
    }

    [Fact]
    public async Task UnFollow_UserDoesNotExist_ThrowsException()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. follow");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => authorRepo.UnFollow("mr. test", author.UserName!));
    }

    [Fact]
    public async Task UnFollow_UserToUnfollowIsItSelf_ThrowsException()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        context.Authors.Add(author);
        await context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => authorRepo.UnFollow(author.UserName!, author.UserName!));
    }

    [Fact]
    public async Task UnFollow_UserToUnfollowIsNotFollowed_ReturnsFalse()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        
        context.Authors.Add(following);
        context.Authors.Add(author);
        
        await context.SaveChangesAsync();
        
        //Act
        var result = await authorRepo.UnFollow(author.UserName!, following.UserName!);
        
        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UnFollow_UserToUnfollowIsFollowed_ReturnsTrue()
    {
        //Arrange
        var context = fixture.GetContext();
        var authorRepo = new AuthorRepository(context);
        
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        author.Following.Add(following);
        
        context.Authors.Add(following);
        context.Authors.Add(author);
        
        await context.SaveChangesAsync();
        
        //Act
        var result = await authorRepo.UnFollow(author.UserName!, following.UserName!);
        var authorsFollowing = context.Authors
            .Where(a => a.NormalizedUserName == author.NormalizedUserName)
            .SelectMany(a => a.Following).ToList();
        
        //Assert
        Assert.True(result);
        Assert.Empty(authorsFollowing);
    }

    #endregion
}