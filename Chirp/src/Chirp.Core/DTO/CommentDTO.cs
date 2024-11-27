namespace Chirp.Core.DTO;

public class CommentDTO (string author,  int cheepId, string message, long unixTimestamp)
{
    public string Author { get; set; } = author;
    public int CheepId { get; set; } = cheepId;
    public string Message { get; set; } = message;
    public long UnixTimestamp { get; set; } = unixTimestamp;
}