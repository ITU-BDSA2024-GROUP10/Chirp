namespace Chirp.Razor.DataModels;

public class Cheep
{
    private string Text { get; set; }
    private DateTime TimeStamp { get; set; }
    private Author Author { get; set; }

    public Cheep(string text, DateTime timeStamp, Author author)
    {
        Text = text;
        TimeStamp = timeStamp;
        Author = author;
    }
}