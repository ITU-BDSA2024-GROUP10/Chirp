﻿using Chirp.Razor.DataModels;
using Microsoft.EntityFrameworkCore;
using SimpleDB.Model;

namespace SimpleDB;

public class AuthorRepository : IAuthorRepository
{
    private ChirpDBContext context;

    public AuthorRepository(ChirpDBContext context)
    {
        this.context = context;
    }

    public async Task<AuthorDTO?> GetAuthorByName(string name) =>
        await context.Authors
            .Where(a => a.Name == name)
            .Select(a => new AuthorDTO(a.Name, a.Email))
            .FirstOrDefaultAsync();

    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        var query = context.Authors
            .Where(author => author.Email == email)
            .Select(author => new {author.Name, author.Email});
            
        
        
        var authors = await query.ToListAsync();
        
        return authors.Select(author => new AuthorDTO(author.Name, author.Email)).FirstOrDefault();
    }

    public Task<bool> AddAuthor(AuthorDTO author)
    {
        throw new NotImplementedException();
    }
}