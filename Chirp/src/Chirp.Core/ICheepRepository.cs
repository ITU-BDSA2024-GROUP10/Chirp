using Chirp.Core.DTO;

namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<IEnumerable<CheepDTO>?> GetCheepsByPage(int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorByPage(String author, int page, int pageSize);
    public Task<IEnumerable<CheepDTO>> GetCheepsFromAuthorsByPage(IEnumerable<String> authors, int page, int pageSize);
    public Task<bool> CreateCheep(CheepDTO cheep);
}