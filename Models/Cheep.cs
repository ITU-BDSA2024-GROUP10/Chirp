using CsvHelper.Configuration.Attributes;

public class Cheep
{
    public string author { get; set; }
    public string message { get; set; }
    public DateTime date { get; set; }

    public static Cheep CheepFromString(string inputString)
    {
        int last = inputString.LastIndexOf(",");
        int first = inputString.IndexOf(",");

        string name = inputString.Substring(0, first);
        string message = inputString.Substring(first + 2, last - first - 3);
        DateTime date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(inputString.Substring(last + 1))).DateTime;

        return new Cheep(name, message, long.Parse(inputString.Substring(last + 1)));
    }

    public Cheep()
    {
    }

    public Cheep(string author, string message, long unixTime)
    {
        this.author = author;
        this.message = message;
        date = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
    }

    public override string ToString()
    {
        return $"{author} @ {date}: {message}";
    }
}