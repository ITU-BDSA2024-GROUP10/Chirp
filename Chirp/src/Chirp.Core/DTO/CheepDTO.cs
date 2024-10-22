namespace SimpleDB.Model;

public class CheepDTO
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long UnixTimestamp { get; set; }
    
    public CheepDTO(string author, string message, long unixTimestamp)
    {
        Author = author;
        Message = message;
        UnixTimestamp = unixTimestamp;
    }
    
    public CheepDTO(string author, string message, DateTime dateTimeTimestamp)
    {
        Author = author;
        Message = message;
        UnixTimestamp = new DateTimeOffset(dateTimeTimestamp).ToUnixTimeSeconds();
    }
}