namespace Chirp.Core.DTO;

public class LikeDTO(string author, int cheepId)
{
    public string Author { get; set; } = author;
    public int CheepId { get; set; } = cheepId;
}