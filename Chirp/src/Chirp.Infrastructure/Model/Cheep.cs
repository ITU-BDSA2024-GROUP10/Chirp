using FluentValidation;

namespace Chirp.Infrastructure.Model;
/// <summary>
/// The Cheep class represents the functionality of a Cheep; a 160-character long message that can be posted on the Chirp! application by a given Author.
/// The Cheep can also be commented on and liked by an Author
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