namespace Chirp.Core.DTO;

public class CheepDTO(int? id, string author, string message, long unixTimestamp)
{
    public int? Id { get; set; } = id;
    public string Author { get; set; } = author;
    public string Message { get; set; } = message;
    public long UnixTimestamp { get; set; } = unixTimestamp;
    public int LikeCount { get; set; } = 0;
}