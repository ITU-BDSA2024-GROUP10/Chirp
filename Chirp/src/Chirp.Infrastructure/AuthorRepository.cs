﻿using Chirp.Razor.DataModels;
using SimpleDB.Model;

namespace SimpleDB;

public class AuthorRepository : IAuthorRepository
{
    private ChirpDBContext context;
    public AuthorRepository(ChirpDBContext context)
    {
        this.context = context;
    } 
    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        throw new NotImplementedException();
    }
    
    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddAuthor(AuthorDTO author)
    {
        throw new NotImplementedException();
    }
    
}