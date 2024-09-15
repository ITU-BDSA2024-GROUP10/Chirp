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

    /*[Fact]
    public void Store_NonExsitingFile_ShouldWriteHeaderAndRecord()
    {
        //Arrange
        CsvTestObject data1 = new ("test1", 1, DateTime.Now, true);
        CsvTestObject data2 = new ("test2", 2, DateTime.Now.AddHours(-1234), false);
        CsvTestObject data3 = new ("test3", 3, DateTime.Now.AddHours(1234), true);
        
        fixture.mockCsvWriter.Setup(c => c.WriteHeader<CsvTestObject>());
        fixture.mockCsvWriter.Setup(c => c.WriteRecord(data1));
        fixture.mockCsvWriter.Setup(c => c.WriteRecord(data2));
        fixture.mockCsvWriter.Setup(c => c.WriteRecord(data3));
        
        //Act
        fixture.csvDatabase.Store(data1);
        fixture.csvDatabase.Store(data2);
        fixture.csvDatabase.Store(data3);
        
        //Asert
        //fixture.mockCsvWriter.Verify(c => c.WriteHeader<CsvTestObject>(), Times.Once);
        fixture.mockCsvWriter.Verify(c => c.WriteRecord(data1), Times.Once);
        fixture.mockCsvWriter.Verify(c => c.WriteRecord(data2), Times.Once);
        fixture.mockCsvWriter.Verify(c => c.WriteRecord(data3), Times.Once);
    }*/

}