
using CsvHelper.Configuration;

public class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(m => m.author).Name("Author");
        Map(m => m.message).Name("Message");
        Map(m => m.date).Name("Timestamp");
    }
}