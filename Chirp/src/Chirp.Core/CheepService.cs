using Chirp.Core.DTO;

namespace Chirp.Core;

public interface ICheepService
{
    public List<CheepDTO> GetCheepsByPage(int page, int pageSize);
    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize);
    public List<CheepDTO> GetCheepsFromAuthorsByPage(IEnumerable<string> authors, int page, int pageSize);
    public int GetAmountOfCheepPages(int pageSize);
    public int GetAmountOfCheepPagesFromAuthors(IEnumerable<String> authors, int pageSize);
    public bool CreateCheep(CheepDTO cheep);
}

public class CheepService(ICheepRepository db) : ICheepService
{
    public List<CheepDTO> GetCheepsByPage(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pageSize);

        var result = db.GetCheepsByPage(page, pageSize).Result ?? throw new ArgumentNullException(nameof(db.GetCheepsByPage));
        
        return result.ToList();
    }

    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorByPage(author, page, pageSize).Result.ToList();
    }
    
    public List<CheepDTO> GetCheepsFromAuthorsByPage(IEnumerable<string> authors, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorsByPage(authors, page, pageSize).Result.ToList();
    }

    public int GetAmountOfCheepPages(int pageSize)
    {
        return db.GetAmountOfCheeps().Result / pageSize;
    }

    public int GetAmountOfCheepPagesFromAuthors(IEnumerable<String> authors, int pageSize)
    {
        return db.GetAmountOfCheepsFromAuthors(authors).Result / pageSize;
    }

    public bool CreateCheep(CheepDTO cheep)
    {
        return db.CreateCheep(cheep).Result;
    }
}