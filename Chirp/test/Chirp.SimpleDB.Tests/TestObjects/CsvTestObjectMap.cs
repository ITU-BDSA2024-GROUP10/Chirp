using CsvHelper.Configuration;

namespace Chirp.SimpleDB.Tests;

public class CsvTestObjectMap : ClassMap<CsvTestObject>
{
    public CsvTestObjectMap()
    {
        Map(m => m.field1String).Index(0).Name("field1");
        Map(m => m.field2Int).Index(1).Name("field2");
        Map(m => m.field3DataTime).Index(2).Name("field3");
        Map(m => m.field4Bool).Index(2).Name("field4");
    }
}