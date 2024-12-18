using FluentValidation;

namespace Chirp.Infrastructure.Model;
/// <summary>
/// The Cheep class, represent a cheep, which is a small 160-character message that can
/// be published by authors on the Chirp! platform,
/// as well as commented and liked by authors
/// </summary>
public class Cheep
{
    public int Id { get; set; }
    public required string Message { get; set; }
    public DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    public List<Comment> Comments { get; set; } = [];
    public List<Like> Likes { get; set; } = [];

    public Cheep()
    {
    }
    /// <summary>
    /// Constructor to instantiate a new Cheep with the relevant information
    /// </summary>
    /// <param name="id"></param>
    /// <param name="message"></param>
    /// <param name="timeStamp"></param>
    /// <param name="author"></param>
    public Cheep(int id, string message, DateTime timeStamp, Author author)
    {
        Id = id;
        Message = message;
        TimeStamp = timeStamp;
        Author = author;
    }
}