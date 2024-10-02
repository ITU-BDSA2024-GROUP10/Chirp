using SimpleDB;
using SimpleDB.Model;

public interface ICheepService
{
    public List<CheepDTO> GetCheepsByPage(int page, int pageSize);
    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize);
}

public class CheepService : ICheepService
{
    private IDatabaseRepository<CheepDTO> db = new SQLiteDBFascade();
    
    public List<CheepDTO> GetCheepsByPage(int page, int pageSize)
    {
        return db.GetByPage(page, pageSize).ToList();
    }
    
    public List<CheepDTO> GetCheepsFromAuthorByPage(string author, int page, int pageSize)
    {
        return db.GetFromAuthorByPage(author, page, pageSize).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
