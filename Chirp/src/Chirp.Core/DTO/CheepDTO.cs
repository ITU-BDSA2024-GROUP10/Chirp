namespace Chirp.Core.DTO;

public class CheepDTO(string author, string message, long unixTimestamp)
{
    public string Author { get; set; } = author;
    public string Message { get; set; } = message;
    public long UnixTimestamp { get; set; } = unixTimestamp;
}