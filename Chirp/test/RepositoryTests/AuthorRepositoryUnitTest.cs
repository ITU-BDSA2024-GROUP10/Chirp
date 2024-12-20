﻿using Chirp.Core;
using Chirp.Core.CustomException;
using Chirp.Core.DTO;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Duende.IdentityServer.Extensions;
using RepositoryTests.Utils;
using TestUtilities;

namespace RepositoryTests;

public class AuthorRepositoryUnitTest
    : IClassFixture<InMemoryDBFixture<ChirpDBContext>>
{
    protected ChirpDBContext Context { get; }
    protected IAuthorRepository AuthorRepository { get; }
    
    
    public AuthorRepositoryUnitTest(InMemoryDBFixture<ChirpDBContext> fixture)
    {
        fixture.ResetDatabase();
        Context = fixture.GetContext();
        AuthorRepository = new AuthorRepository(fixture.GetContext());
    }

    [Fact]
    public async Task GetComments_ReturnsComments()
    {
        var author = TestUtils.CreateTestAuthor("test");
        var cheep = new Cheep
        {
            Author = author, 
            Message = "test", 
            Id = 1, 
            TimeStamp = DateTime.Now
        };
        var comment = new Comment
        { Author = author, 
            Message = "comment", 
            Id = 1, 
            Cheep = cheep, 
            TimeStamp = DateTime.Now 
        };
        cheep.Comments.Add(comment);
        author.Comments.Add(comment);
        Context.Cheeps.Add(cheep);
        Context.Comments.Add(comment);
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();
        var comments = await AuthorRepository.GetComments(author.UserName!);
        var first = comments.First();
        Assert.NotNull(comments);
        Assert.Equal(comment.Message,first.Message);
    }

    #region GetAuthorFollows tests

    [Fact]
    public async Task GetAuthorFollows_UserDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<UserDoesNotExist>(() => AuthorRepository.GetAuthorFollows("mr. test"));
    }

    [Fact]
    public async Task GetAuthorFollows_UserExists_ReturnsListOfAuthors()
    {
        #region Arrange

        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        var notFollowing = TestUtils.CreateTestAuthor("mr. not follow");
        
        author.Following.Add(following);

        Context.Authors.Add(notFollowing);
        Context.Authors.Add(following);
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();

        #endregion

        //Act
        var result = await AuthorRepository.GetAuthorFollows(author.UserName!);
        
        //Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal(following.UserName, result.First().Name);
    }

    #endregion

    #region Follow tests

    [Fact]
    public async Task Follow_UserToFollowDoesNotExist_ThrowsException()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => AuthorRepository.Follow(author.UserName!, "mr. follow"));
    }

    [Fact]
    public async Task Follow_UserDoesNotExist_ThrowsException()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. follow");
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => AuthorRepository.Follow("mr. test", author.UserName!));
    }

    [Fact]
    public async Task Follow_UserToFollowIsItSelf_ThrowsException()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => AuthorRepository.Follow(author.UserName!, author.UserName!));
    }

    [Fact]
    public async Task Follow_UserToFollowAlreadyFollowed_ReturnsFalse()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        author.Following.Add(following);
        
        Context.Authors.Add(following);
        Context.Authors.Add(author);
        
        await Context.SaveChangesAsync();
        
        //Act
        var result = await AuthorRepository.Follow(author.UserName!, following.UserName!);
        
        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Follow_UserToFollowIsNotFollowed_ReturnsTrue()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        
        Context.Authors.Add(following);
        Context.Authors.Add(author);
        
        await Context.SaveChangesAsync();
        
        //Act
        var result = await AuthorRepository.Follow(author.UserName!, following.UserName!);
        var authorsFollowing = Context.Authors
            .Where(a => a.NormalizedUserName == author.NormalizedUserName)
            .SelectMany(a => a.Following).ToList();
        
        //Assert
        Assert.True(result);
        Assert.NotEmpty(authorsFollowing);
        Assert.Single(authorsFollowing);
        Assert.Equal(following.UserName, authorsFollowing[0].UserName);
        
    }
    
    [Fact]
    public async Task Follow_MultipelAuthorsFollowTheSameAuthor_ReturnsTrue()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        var following2 = TestUtils.CreateTestAuthor("mr. follow2");
        
        Context.Authors.Add(following);
        Context.Authors.Add(following2);
        Context.Authors.Add(author);
        
        await Context.SaveChangesAsync();
        
        //Act
        var result = await AuthorRepository.Follow(following.UserName!, author.UserName!);
        var result2 = await AuthorRepository.Follow(following2.UserName!, author.UserName!);
        var authorsFollowers = Context.Authors
            .Where(a => a.NormalizedUserName == author.NormalizedUserName)
            .SelectMany(a => a.Followers).ToList();
        
        //Assert
        Assert.True(result);
        Assert.True(result2);
        Assert.NotEmpty(authorsFollowers);
        Assert.Equal(2, authorsFollowers.Count);
        Assert.Contains(authorsFollowers, a => a.UserName == following.UserName);
        Assert.Contains(authorsFollowers, a => a.UserName == following2.UserName);
    }
    
    #endregion

    #region UnFollow tests

    [Fact]
    public async Task UnFollow_UserToUnfollowDoesNotExist_ThrowsException()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => AuthorRepository.UnFollow(author.UserName!, "mr. follow"));
    }

    [Fact]
    public async Task UnFollow_UserDoesNotExist_ThrowsException()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. follow");
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<UserDoesNotExist>(() => AuthorRepository.UnFollow("mr. test", author.UserName!));
    }

    [Fact]
    public async Task UnFollow_UserToUnfollowIsItSelf_ThrowsException()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        Context.Authors.Add(author);
        await Context.SaveChangesAsync();
        
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => AuthorRepository.UnFollow(author.UserName!, author.UserName!));
    }

    [Fact]
    public async Task MakeFollowersUnfollow_RemovesFollowers_ReturnsTrue()
    {
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        
        following.Following.Add(author);
        Context.Authors.Add(following);
        Context.Authors.Add(author);
        
        await Context.SaveChangesAsync();
        
        var result = await AuthorRepository.MakeFollowersUnfollow(author.UserName!);

        var authorsFollowing = Context.Authors
            .Where(a => a.NormalizedUserName == following.NormalizedUserName)
            .SelectMany(a => a.Following).ToList();
        
        Assert.True(authorsFollowing.IsNullOrEmpty());
        
        Assert.True(result);
        
    }

    [Fact]
    public async Task UnFollow_UserToUnfollowIsNotFollowed_ReturnsFalse()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        
        Context.Authors.Add(following);
        Context.Authors.Add(author);
        
        await Context.SaveChangesAsync();
        
        //Act
        var result = await AuthorRepository.UnFollow(author.UserName!, following.UserName!);
        
        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UnFollow_UserToUnfollowIsFollowed_ReturnsTrue()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        author.Following.Add(following);
        
        Context.Authors.Add(following);
        Context.Authors.Add(author);
        
        await Context.SaveChangesAsync();
        
        //Act
        var result = await AuthorRepository.UnFollow(author.UserName!, following.UserName!);
        var authorsFollowing = Context.Authors
            .Where(a => a.NormalizedUserName == author.NormalizedUserName)
            .SelectMany(a => a.Following).ToList();
        
        //Assert
        Assert.True(result);
        Assert.Empty(authorsFollowing);
    }

    #endregion
    
    #region GetAuthorsByNames tests
    
    [Fact]
    public async Task GetAuthorsByNames_NoAuthorsExist_ReturnsEmptyList()
    {
        //Arrange
        var names = new List<string> { "mr. test", "mr. follow", "mr. not follow" };
        
        //Act
        var result = await AuthorRepository.GetAuthorsByNames(names);
        
        //Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetAuthorsByNames_AuthorsExist_ReturnsListOfAuthors()
    {
        //Arrange
        var author = TestUtils.CreateTestAuthor("mr. test");
        var following = TestUtils.CreateTestAuthor("mr. follow");
        var notFollowing = TestUtils.CreateTestAuthor("mr. not follow");
        
        Context.Authors.Add(notFollowing);
        Context.Authors.Add(following);
        Context.Authors.Add(author);
        
        await Context.SaveChangesAsync();
        
        var names = new List<string> { "mr. test", "mr. follow", "mr. not follow" };
        
        //Act
        var result = await AuthorRepository.GetAuthorsByNames(names);
        
        //Assert
        Assert.NotNull(result);
        var authorDtos = result as AuthorDTO?[] ?? result.ToArray();
        Assert.NotEmpty(authorDtos);
        Assert.Equal(3, authorDtos.Count());
        Assert.Contains(authorDtos, a => a!.Name == author.UserName);
        Assert.Contains(authorDtos, a => a!.Name == following.UserName);
        Assert.Contains(authorDtos, a => a!.Name == notFollowing.UserName);
    }
    
    #endregion
}