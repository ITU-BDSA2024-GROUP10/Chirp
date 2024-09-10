using SimpleDB;
using CsvHelper;
using CsvHelper.Configuration;
using System.Text;
using System.Globalization;

namespace Chirp.CLI.SimpleDB;
public class CSVDatabase<T> : IDatabaseRepository<T>
{   
    readonly String _fileName;
    readonly CsvConfiguration _config;
    readonly ClassMap<T> _classMap;

    public CSVDatabase(string fileName, ClassMap<T> classMap) 
    {
        _fileName = fileName;
        _classMap = classMap;
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.UTF8
        };
    }

    public IEnumerable<Cheep> Read(int? limit = null)
    {
        List<Cheep> elements = new();
        using (var reader = new StreamReader(_fileName, Encoding.UTF8))
        using (var csv = new CsvReader(reader, _config))
        {
            csv.Context.RegisterClassMap(_classMap);
            csv.Read();
            csv.ReadHeader();
            elements = csv.GetRecords<Cheep>().ToList();
        }
        
        int amountToRead = Math.Min(elements.Count, limit ?? elements.Count); // get or default to elements.Count

        return elements.GetRange(elements.Count - amountToRead, amountToRead);
    }

    public void Store(Cheep record)
    {
        List<T> elements = new();
        using (var reader = new StreamReader(fileName, Encoding.UTF8))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap(classMap);
            csv.Read();
            csv.ReadHeader();
            elements = csv.GetRecords<T>().ToList();
        }
        
        amountToRead = Math.Min(elements.Count, amountToRead);
        return elements.GetRange(elements.Count-amountToRead, amountToRead);
    }
}
