namespace Chirp.Core.DTO;

public class LikeDTO(string author, int cheepId, int? id = null)
{
    public int? Id { get; set; } = id;
    public string Author { get; set; } = author;
    public int CheepId { get; set; } = cheepId;
}