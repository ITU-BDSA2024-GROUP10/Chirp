using System.Diagnostics.CodeAnalysis;

namespace Chirp.Razor.DataModels;

public class Cheep
{
    public int Id { get; set;  } 
    public required string Message { get; set; } 
    public DateTime TimeStamp { get; set; } 
    public required Author Author { get; set; }
    
    public Cheep()
    {
        
    }

    public Cheep(int id, string message, DateTime timeStamp, Author author)
    {
        Id = id;
        Message = message;
        TimeStamp = timeStamp;
        Author = author;
    }
}