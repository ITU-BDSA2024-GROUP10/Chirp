using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CheepCsvDatabase : CsvDatabase<Cheep>
{
    private static readonly Lazy<CheepCsvDatabase> Lazy = new (() => new CheepCsvDatabase());
    
    public static CheepCsvDatabase Instance => Lazy.Value;

    private CheepCsvDatabase() : base("chirp_cli_db.csv", new CheepMap())
    {
    }
}