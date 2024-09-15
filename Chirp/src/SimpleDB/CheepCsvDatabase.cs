using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CheepCsvDatabase : CsvDatabase<Cheep>
{
    private static readonly Lazy<CheepCsvDatabase> Lazy = new (() => new CheepCsvDatabase());
    
    public static CheepCsvDatabase Instance { get { return Lazy.Value; } }

    private CheepCsvDatabase() : base("data/chirp_cli_db.csv", (new CheepMap() as ClassMap<Cheep>)!)
    {
    }
}