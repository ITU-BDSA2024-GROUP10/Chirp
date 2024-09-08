
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
    
    
    public List<T> Read(int amountToRead)
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