﻿// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

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
        Cheep(args[1]);
    }
}


void Read()
{
    List<Chirp> chirps = new();
    var lines = ReadFile();
    foreach (var line in lines)
    {
        if (!String.IsNullOrEmpty(line))
        {
            chirps.Add(Chirp.ChirpFromString(line));
        }
    }

    foreach (var chirp in chirps)
    {
        Console.WriteLine(chirp);
    }
}

void Cheep(string message)
{
    string userName = Environment.UserName;
    string currentTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
    string outputMessage = $"{userName},\"{message}\",{currentTime}";
    
    StreamWriter writer = new StreamWriter("chirp_cli_db.csv", true);
    writer.WriteLine(outputMessage);
    writer.Close();
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

class Chirp
{
    string _name { get; }
    string _message { get; }
    DateTimeOffset _date { get; }

    public static Chirp ChirpFromString(string inputString)
    {
        int last = inputString.LastIndexOf(",");
        int first = inputString.IndexOf(",");
        
        string name = inputString.Substring(0,first);
        string message = inputString.Substring(first+2, last-first-3);
        DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(inputString.Substring(last+1)));
        
        return new Chirp(name, message, date);
    }
    
    public Chirp(string name, string message, DateTimeOffset date)
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

