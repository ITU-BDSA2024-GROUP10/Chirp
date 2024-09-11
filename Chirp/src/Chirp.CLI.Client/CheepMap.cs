using CsvHelper.Configuration;

namespace SimpleDB;

public class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(m => m.author).Index(0).Name("Author");
        Map(m => m.message).Index(1).Name("Message");
        Map(m => m.date).Index(2).Name("Date");
    }
}