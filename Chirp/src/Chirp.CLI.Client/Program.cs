// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using Chirp.CLI.Client;
using DocoptNet;
using SimpleDB;

const string usage = @"Chirp CLI version.

Usage:
    chirp read [<limit>]
    chirp cheep <message>
    chirp (-h | --help)
    chirp --version

Options:
    -h --help   Show this screen.
    --version   Show version.
";

var db = CheepCsvDatabase.Instance;
var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

if (arguments["read"].IsTrue)
{
    if (arguments["<limit>"].IsNullOrEmpty) WebDisplayCheeps();
    else WebDisplayCheeps(arguments["<limit>"].AsInt);
}
else if (arguments["cheep"].IsTrue) 
{
    WebWriteCheep(arguments["<message>"].ToString());
}

return;

void WriteCheep(IDatabaseRepository<Cheep> dbr, string message)
{
    try
    {
        var author = Environment.UserName;
        var time = DateTime.Now;
        var cheep = new Cheep(author, message, time);
        dbr.Store(cheep);
        Console.WriteLine("Cheep stored successfully.");
    }
    catch (FileNotFoundException)
    {
        Console.Error.WriteLine("ERROR: Unable to store the cheep because the db file was not found.");
    }
    catch (InvalidOperationException)
    {
        Console.Error.WriteLine("ERROR: Unable to store the cheep due to an issue with saving the file. Check the file format?");
    }
    catch (Exception)
    {
        Console.Error.WriteLine("ERROR: An unexpected error occurred while storing the cheep.");
    }
}

void DisplayCheeps(IDatabaseRepository<Cheep> dbr, int limit)
{
    try
    {
        var cheeps = dbr.Read(limit);
        UserInterface.PrintCheeps(cheeps);
    }
    catch (FileNotFoundException)
    {
        Console.Error.WriteLine("ERROR: Unable to display cheeps because the data file was not found.");
    }
    catch (InvalidOperationException)
    {
        Console.Error.WriteLine("ERROR: Unable to display cheeps due to an issue with reading the file. Check the file format?");
    }
    catch (Exception)
    {
        Console.Error.WriteLine("ERROR: An unexpected error occurred while displaying cheeps.");
    }
}

void WebWriteCheep(string message)
{
    var author = Environment.UserName;
    var time = DateTime.Now;
    var cheep = new Cheep(author, message, time);
    var baseUrl = "https://bdsa2024group10chirpremotedb-h3c8bne5cahweegw.northeurope-01.azurewebsites.net/";
    using HttpClient client = new();
    client.BaseAddress = new Uri(baseUrl);
    var response = client.PostAsJsonAsync("/cheep", cheep).Result;
    response.EnsureSuccessStatusCode();
}

void WebDisplayCheeps(int? limit = null)
{
    var baseUrl = "https://bdsa2024group10chirpremotedb-h3c8bne5cahweegw.northeurope-01.azurewebsites.net/";
    using HttpClient client = new();
    client.BaseAddress = new Uri(baseUrl);
    var requestUri = $"/cheeps?limit={limit}";
    
    if (limit == null) requestUri = $"/cheeps";
    else requestUri = $"/cheeps?limit={limit}";
    
    var response = client.GetFromJsonAsync<Cheep[]>(requestUri).Result;
    if (response == null) throw new Exception("No response");
    UserInterface.PrintCheeps(response);
}