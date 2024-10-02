namespace Chirp.Razor.DataModels;

public class Cheep
{
    private string text;
    private DateTime timeStamp;
    private Author author;

    public Cheep(string text, DateTime timeStamp, Author author)
    {
        this.text = text;
        this.timeStamp = timeStamp;
        this.author = author;
    }
}