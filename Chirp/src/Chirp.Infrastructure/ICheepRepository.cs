﻿using SimpleDB.DTO;

namespace Chirp.Infrastructure;

public interface ICheepRepository
{
    public Task<IEnumerable<CheepDTO>> GetCheepsByPage(int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(String author, int page, int pageSize);
    public Task<bool> CreateCheep(CheepDTO cheep);
}
