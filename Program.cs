// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.RegularExpressions;
using Chirp.CLI;
using CsvHelper;
using CsvHelper.Configuration;

if (args.Length == 0)
{
    Console.WriteLine("Incorrect arguments! Try: ");
    Console.WriteLine("* dotnet run -- read");
    Console.WriteLine("* dotnet run -- cheep \"message\"");
}
else
{
    if (args[0] == "read")
    {
        Read();
    } else if (args[0] == "cheep")
    {
        WriteCheep(args[1]);
    }
}


void Read()
{
    List<Cheep> cheeps = new();
    var lines = ReadFile();
    foreach (var line in lines)
    {
        if (!String.IsNullOrEmpty(line))
        {
            cheeps.Add(Cheep.CheepFromString(line));
        }
    }

    foreach (var cheep in cheeps)
    {
        Console.WriteLine(cheep);
    }
}

void WriteCheep(string message)
{
    string userName = Environment.UserName;
    DateTimeOffset currentTime = DateTimeOffset.Now.DateTime;
    var records = new List<Cheep>
    {
        new (userName, message, currentTime)
    };
    
    using (var writer = new StreamWriter("chirp_cli_db.csv", true))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.Context.RegisterClassMap<CheepMap>();
        csv.WriteRecords(records);
        csv.NextRecord();
        writer.Flush();
    }
}

List<string> ReadFile()
{
    try
    {
        StreamReader reader = new("chirp_cli_db.csv");
        Console.WriteLine("the program running in chirp_cli_db.csv");
        reader.ReadLine();
        List<string> lines = reader.ReadToEnd().Split('\n').ToList();
        reader.Close();
        
        return lines;

    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
        return null;
    }
    

}

public record Cheep 
{
    public string _name { get; }
    public string _message { get; }
    public DateTimeOffset _date { get; }

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

public class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(m => m._name).Index(0).Name("name");
        Map(m => m._message).Index(1).Name("message");
        Map(m => m._date).Index(2).Name("date");
    }
}

