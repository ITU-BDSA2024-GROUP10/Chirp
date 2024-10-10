﻿using Chirp.Razor.DataModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using SimpleDB;
using SimpleDB.Model;

namespace RepositoryTests;

public class AuthorRepositoryUnitTest
{
    private readonly SqliteConnection _connection;
    private Mock<IAuthorRepository> mockInitializer()
    {
        var mock = new Mock<IAuthorRepository>();
        return mock;
    }

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
    public void GetAuthorByName_NameCantBeFound_ReturnNull()
    {
        //arrange
        var chirpContext = GetContext();
        var author = new Author {Name = "John Doe", Email = "JohnDoe@gmail.com"};

        chirpContext.Authors.Add(author);
        chirpContext.SaveChanges();

        IAuthorRepository authorRepo = new AuthorRepository(chirpContext);

        //Act
        var result = authorRepo.GetAuthorByName(author.Name);
        
        //Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAuthorByName_NameIsHelge_ReturnsAuthorDTOOfHelge()
    {
        //Arrange an arbitrary author with name 'Helge' and create arbitrary database to put up
        
        //Act a scenario where the repository can get an author by the name of 'Helge'
        
        //Assert the value of the arbitrary author that was arranged
    }
    
    [Fact]
    public void AddAuthor_NameIsNullKeyword_ReturnIllegalArgument()
    {
        //Arrange
        var repositoryMock = new Mock<IAuthorRepository>();
        repositoryMock
            .Setup(r => r.GetAuthorByName(null).Result.Name);

        //act
        
        //Assert
    }

    [Fact]
    public void AddAuthor_NameIsAnEmptyString_ReturnFalse()
    {
        //Arrange an arbitrary author with an empty string as name and make arbitrary DB
        
        //Act a scenario where you want to hook up the name to the DB but will return false because the adding wasn't
        //successful
        
        //AssertBool false
    }
}