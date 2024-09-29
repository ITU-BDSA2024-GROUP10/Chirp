using SimpleDB;
using SimpleDB.Model;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private IDatabaseRepository<CheepViewModel> db = new SQLiteDBFascade();

    public List<CheepViewModel> GetCheeps()
    {
        return db.GetAll().ToList();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return db.GetFromAuthor(author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
