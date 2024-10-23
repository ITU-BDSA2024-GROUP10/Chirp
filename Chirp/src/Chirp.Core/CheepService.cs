using Chirp.Core.DTO;

namespace Chirp.Core;

public interface ICheepService
{
    public List<CheepDTO> GetCheepsByPage(int page, int pageSize);
    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize);
}

public class CheepService(ICheepRepository db) : ICheepService
{
    public List<CheepDTO> GetCheepsByPage(int page, int pageSize)
    {
        return db.GetCheepsByPage(page, pageSize).Result.ToList();
    }

    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorByPage(author, page, pageSize).Result.ToList();
    }
}