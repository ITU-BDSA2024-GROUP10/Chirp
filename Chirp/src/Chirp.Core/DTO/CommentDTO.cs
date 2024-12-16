namespace Chirp.Core.DTO;
/// <summary>
/// The CommentDTO class transfers Comment data from RepositoryLayer to the UI
/// </summary>
public class CommentDTO(string author, int cheepId, string message, long unixTimestamp)
{
    public string Author { get; set; } = author;
    public int CheepId { get; set; } = cheepId;
    public string Message { get; set; } = message;
    public long UnixTimestamp { get; set; } = unixTimestamp;
}