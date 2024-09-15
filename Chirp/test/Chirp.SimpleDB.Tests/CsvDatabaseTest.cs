using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Moq;
using SimpleDB;

namespace Chirp.SimpleDB.Tests;

class CsvDatabaseTextFixture
{
    private readonly string testCsvDatabaseFilePath = "CsvDatabaseUnitTest.csv";

    public readonly Mock<TextWriter> mockTextWriter;
    public readonly Mock<CsvWriter> mockCsvWriter;
    public readonly Mock<TextReader> mockTextReader;
    public readonly Mock<CsvReader> mockCsvReader;
    
    public readonly CsvConfiguration config;
    public readonly CsvDatabase<CsvTestObject> csvDatabase;
    public readonly CsvTestObjectMap classMap;

    
    public CsvDatabaseTextFixture()
    {
        mockTextWriter = new Mock<TextWriter>();
        mockCsvWriter = new Mock<CsvWriter>(mockTextWriter.Object, new CsvConfiguration(CultureInfo.InvariantCulture));
        mockTextReader = new Mock<TextReader>();
        mockCsvReader = new Mock<CsvReader>(mockTextReader.Object, new CsvConfiguration(CultureInfo.InvariantCulture));
        
        config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.UTF8
        };

        classMap = new CsvTestObjectMap();
        csvDatabase = new CsvDatabase<CsvTestObject>(testCsvDatabaseFilePath, classMap);
    }

    public void Dispose()
    {
        if (File.Exists(testCsvDatabaseFilePath))
        {
            File.Delete(testCsvDatabaseFilePath);
        }
    }

    public void SetupTestCsvDatabase()
    {
        using (var writer = new StreamWriter(testCsvDatabaseFilePath, append: true, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap(classMap);
                csv.WriteHeader<CsvTestObject>();
                csv.NextRecord();
            }
    }
}

public class CsvDatabaseTest : IDisposable, IClassFixture<CsvDatabaseTextFixture>
{
    private readonly CsvDatabaseTextFixture fixture;

    public CsvDatabaseTest()
    {
        fixture = new CsvDatabaseTextFixture();
    }
    
    public void Dispose()
    {
        fixture.Dispose();
    }

}