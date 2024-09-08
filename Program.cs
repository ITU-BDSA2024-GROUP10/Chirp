// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;
using DocoptNet;
using Chirp.CLI;

const string usage = @"Chirp CLI version.

Usage:
    chirp read <limit>
    chirp cheep <message>
    chirp (-h | --help)
    chirp --version

Options:
    -h --help   Show this screen.
    --version   Show version.
";

var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

if (arguments["read"].IsTrue) 
{
    CsvHandler<Cheep> csvHandler = new("chirp_cli_db.csv", new CheepMap());
    List<Cheep> cheeps = csvHandler.ReadCheeps();
    foreach (var cheep in cheeps)
    {
        Console.WriteLine(cheep);
    }
    //Read();
} else if (arguments["cheep"].IsTrue)
{
    WriteCheep(arguments["<message>"].ToString());
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
        Console.WriteLine(cheep);
}

void WriteCheep(string message)
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

