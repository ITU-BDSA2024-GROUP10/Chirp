namespace Chirp.Infrastructure.Model;

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

    public Comment(int id, Author author, Cheep cheep, string message, DateTime timeStamp)
    {
        Id = id;
        Author = author;
        Cheep = cheep;
        Message = message;
        TimeStamp = timeStamp;
    }
}