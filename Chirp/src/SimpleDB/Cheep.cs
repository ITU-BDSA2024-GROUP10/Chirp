public class Cheep
{
    public required string Author { get; set; }
    public required string Message { get; set; }
    public DateTime Date { get; set; }

    public Cheep()
    {
    }

    public Cheep(string author, string message, DateTime date)
    {
        this.Author = author;
        this.Message = message;
        this.Date = date;
    }

    public override string ToString()
    {
        return $"{Author} @ {Date}: {Message}";
    }
}