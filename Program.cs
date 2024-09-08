// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.RegularExpressions;
using DocoptNet;
using Chirp.CLI;
using CsvHelper;
using CsvHelper.Configuration;

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

void WriteCheep(string message)
{
    string userName = Environment.UserName;
    DateTime currentTime = DateTimeOffset.Now.DateTime;
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
