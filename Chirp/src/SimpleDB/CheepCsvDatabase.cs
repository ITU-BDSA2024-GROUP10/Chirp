using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CheepCsvDatabase<Cheep> : CsvDatabase<Cheep>
{
    private static readonly Lazy<CheepCsvDatabase<Cheep>> Lazy = new (() => new CheepCsvDatabase<Cheep>());
    
    public static CheepCsvDatabase<Cheep> Instance { get { return Lazy.Value; } }

    private CheepCsvDatabase() : base("data/chirp_cli_db.csv", (new CheepMap() as ClassMap<Cheep>)!)
    {
    }
}