﻿// See https://aka.ms/new-console-template for more information

using Chirp.CLI;
using DocoptNet;
using Chirp.CLI.SimpleDB;

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

var db = new CSVDatabase<Cheep>("chirp_cli_db.csv", new CheepMap());
var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

if (arguments["read"].IsTrue) 
{
    ReadCheeps(arguments);
}
else if (arguments["cheep"].IsTrue) 
{
    WriteCheep(arguments["<message>"].ToString());
}

void WriteCheep(string message)
{
    var author = Environment.UserName;
    var time = DateTime.Now;
    var cheep = new Cheep(author, message, time);
    db.Store(cheep);
}

void ReadCheeps(IDictionary<string, ValueObject> arguments)
{
    List<Cheep> cheeps = (List<Cheep>)db.Read(arguments["<limit>"].AsInt);

    UserInterface.PrintCheeps(cheeps);
}