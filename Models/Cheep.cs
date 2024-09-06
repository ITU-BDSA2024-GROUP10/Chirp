public class Cheep
{
    string _name { get; }
    string _message { get; }
    DateTimeOffset _date { get; }

    public static Cheep CheepFromString(string inputString)
    {
        int last = inputString.LastIndexOf(",");
        int first = inputString.IndexOf(",");
        
        string name = inputString.Substring(0,first);
        string message = inputString.Substring(first+2, last-first-3);
        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(inputString.Substring(last+1)));

        return new Cheep(name, message, date);
    }
    
    public Cheep(string name, string message, DateTimeOffset date)
    {
        _name = name;
        _message = message;
        _date = date;
		 }
    
    public override string ToString()
    {
        return $"{_name} @ {_date.DateTime}: {_message}";
    }
}