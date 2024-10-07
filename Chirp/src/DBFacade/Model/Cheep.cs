namespace Chirp.Razor.DataModels;

public class Cheep
{
    public int Id { get; set;  } 
    public string Text { get; set; } 
    public DateTime TimeStamp { get; set; } 
    
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    
    public Cheep()
    {
        
    }

    public Cheep(int id, string text, DateTime timeStamp, Author author)
    {
        Id = id;
        Text = text;
        TimeStamp = timeStamp;
        Author = author;
    }
}