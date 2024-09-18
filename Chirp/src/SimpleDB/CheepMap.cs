using CsvHelper.Configuration;

namespace SimpleDB;

public class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(m => m.Author).Index(0).Name("Author");
        Map(m => m.Message).Index(1).Name("Message");
        Map(m => m.Date).Index(2).Name("Date");
    }
}