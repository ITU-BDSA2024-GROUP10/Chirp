﻿using Chirp.Razor.DataModels;
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
    public async void AddAuthor_NameIsAnEmptyString_ReturnFalse()
    {
        var chirpContext = GetContext();
        var author = new AuthorDTO("", "@gmail.com");

        IAuthorRepository authorRepository = new AuthorRepository(chirpContext);
        
        //Act
        var result = await authorRepository.AddAuthor(author);
        
        //Assert
        Assert.False(result);
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
        
        //Assert
        Assert.True(result);
    }

    public void Dispose()
    {
        _connection.Close();
    }
}