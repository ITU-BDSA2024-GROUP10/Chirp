using Chirp.Core.DTO;

namespace Chirp.Core;

public interface ICheepService
{
    public List<CheepDTO> GetCheepsByPage(int page, int pageSize);
    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize);
}

public class CheepService(ICheepRepository db) : ICheepService
{
    private readonly ICheepRepository db = db;

    public List<CheepDTO> GetCheepsByPage(int page, int pageSize)
    {
        return db.GetCheepsByPage(page, pageSize).Result.ToList();
    }
    
    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return db.GetCheepsFromAuthorByPage(author, page, pageSize).Result.ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
