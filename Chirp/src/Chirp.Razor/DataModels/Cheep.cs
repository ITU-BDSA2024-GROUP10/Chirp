namespace Chirp.Razor.DataModels;

public class Cheep
{
    private int Id { get; set;  }
    private string Text { get; set; }
    private DateTime TimeStamp { get; set; }
    private Author Author { get; set; }

    public Cheep(string text, DateTime timeStamp, Author author)
    {

    public Cheep(int id, string text, DateTime timeStamp, Author author)
    {
        Id = id;
        Text = text;
        TimeStamp = timeStamp;
        Author = author;
    }
}