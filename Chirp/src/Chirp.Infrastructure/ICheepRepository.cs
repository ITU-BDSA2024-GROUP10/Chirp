using Chirp.Razor.DataModels;
using SimpleDB.Model;

namespace SimpleDB;

public interface ICheepRepository
{
    public Task<IEnumerable<CheepDTO>> GetCheepsByPage(int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(String author, int page, int pageSize);
    public void CreateCheep(CheepDTO cheep);
}
