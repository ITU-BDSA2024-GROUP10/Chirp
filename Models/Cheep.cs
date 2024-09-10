using CsvHelper.Configuration.Attributes;

public class Cheep
{
    public string author { get; set; }
    public string message { get; set; }
    public DateTime date { get; set; }

    public Cheep()
    {
    }

    public Cheep(string author, string message, DateTime date)
    {
        this.author = author;
        this.message = message;
        this.date = date;
    }

    public override string ToString()
    {
        return $"{author} @ {date}: {message}";
    }
}