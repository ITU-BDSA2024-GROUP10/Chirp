namespace Chirp.Infrastructure.Model;
/// <summary>
/// The Comment class is used to associate Comments made by Authors on Cheeps
/// </summary>
public class Comment
{
    public int Id { get; set; }
    public required Author Author { get; set; }
    public required Cheep Cheep { get; set; }
    public DateTime TimeStamp { get; set; }
    public required string Message { get; set; }

    public Comment()
    {
    }
    /// <summary>
    /// Constructor for creating a new Comment object with all relevant information
    /// </summary>
    /// <param name="id"></param>
    /// <param name="author"></param>
    /// <param name="cheep"></param>
    /// <param name="message"></param>
    /// <param name="timeStamp"></param>
    public Comment(int id, Author author, Cheep cheep, string message, DateTime timeStamp)
    {
        Id = id;
        Author = author;
        Cheep = cheep;
        Message = message;
        TimeStamp = timeStamp;
    }
}