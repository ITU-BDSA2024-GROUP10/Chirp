
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

public class CsvHandler<T>
{
    private readonly CsvConfiguration config;
    private readonly ClassMap<T> classMap;
    private readonly string fileName;
    
    public CsvHandler(string fileName, ClassMap<T> classMap)
    {
        this.fileName = fileName;
        this.classMap = classMap;
        
        config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.UTF8
        };
    }
    
    
    public List<T> ReadCheeps()
    {
        List<T> cheeps = new();
        using (var reader = new StreamReader(fileName, Encoding.UTF8))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap(classMap);
            csv.Read();
            csv.ReadHeader();
            cheeps = csv.GetRecords<T>().ToList();
        }
        return cheeps;
    }
}