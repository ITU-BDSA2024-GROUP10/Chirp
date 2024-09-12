// See https://aka.ms/new-console-template for more information

using Chirp.CLI.Client;
using DocoptNet;
using SimpleDB;

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

var db = new CsvDatabase<Cheep>("data/chirp_cli_db.csv", new CheepMap());
var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

if (arguments["read"].IsTrue) 
{
    DisplayCheeps(db, arguments["<limit>"].AsInt);
}
else if (arguments["cheep"].IsTrue) 
{
    WriteCheep(db, arguments["<message>"].ToString());
}

return;

void WriteCheep(IDatabaseRepository<Cheep> dbr, string message)
{
    var author = Environment.UserName;
    var time = DateTime.Now;
    var cheep = new Cheep(author, message, time);
    dbr.Store(cheep);
}

void DisplayCheeps(IDatabaseRepository<Cheep> dbr, int limit)
{
    var cheeps = dbr.Read(limit);
    UserInterface.PrintCheeps(cheeps);
}