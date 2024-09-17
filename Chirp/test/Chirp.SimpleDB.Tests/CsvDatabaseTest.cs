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

    public void CreateTestCsvDatabaseFile()
    {
        using (var writer = new StreamWriter(testCsvDatabaseFilePath, append: true, Encoding.UTF8))
        {
            writer.WriteLine("field1,field2,field3,field4");
        }
    }
    
    public void WriteTestDataToCsvDatabase(List<CsvTestObject> testData)
    {
        using (var writer = new StreamWriter(testCsvDatabaseFilePath, append: true, Encoding.UTF8))
        {
            foreach (var data in testData)
            {
                writer.WriteLine(data.ToString());
            }
        }
    }

    public List<CsvTestObject> ExtractDataFromCsvDatabase()
    {
        List<CsvTestObject> result = new ();

        using (var reader = new StreamReader(testCsvDatabaseFilePath, Encoding.UTF8))
        {
            reader.ReadLine(); 
            var lines = reader.ReadToEnd().Split("\n");
            foreach (var line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                result.Add(new CsvTestObject(line.Split(",")));
            }
        }

        return result;
    }
    
    public bool HasHeader()
    {
        using (var reader = new StreamReader(testCsvDatabaseFilePath, Encoding.UTF8))
        {
            return reader.ReadLine().Equals("field1,field2,field3,field4");
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

    [Fact]
    public void Integration_NonExsistingFileReadingAll_ShouldBeAbelToWriteAndRead()
    {
        //Arrange
        CsvTestObject data1 = new ("test1", 1, DateTime.Now, true);
        CsvTestObject data2 = new ("test2", 2, DateTime.Now.AddHours(-1234), false);
        CsvTestObject data3 = new ("test3", 3, DateTime.Now.AddHours(1234), true);

        List<CsvTestObject> result = new ();
        
        //Act
        fixture.csvDatabase.Store(data1);
        fixture.csvDatabase.Store(data2);
        fixture.csvDatabase.Store(data3);

        result = fixture.csvDatabase.Read(3).ToList();

        //Asert
        Assert.Equal(3, result.Count);
        Assert.True(result[0].Equals(data1));
        Assert.True(result[1].Equals(data2));
        Assert.True(result[2].Equals(data3));
        
        fixture.Dispose();
    }
    
    [Fact]
    public void Integration_ExsistingFileReadingAll_ShouldBeAbelToWriteAndRead()
    {   
        //Arrange
        fixture.SetupTestCsvDatabase();
        
        CsvTestObject data1 = new ("test1", 1, DateTime.Now, true);
        CsvTestObject data2 = new ("test2", 2, DateTime.Now.AddHours(-1234), false);
        CsvTestObject data3 = new ("test3", 3, DateTime.Now.AddHours(1234), true);

        List<CsvTestObject> result = new ();
        
        //Act
        fixture.csvDatabase.Store(data1);
        fixture.csvDatabase.Store(data2);
        fixture.csvDatabase.Store(data3);

        result = fixture.csvDatabase.Read(3).ToList();

        //Asert
        Assert.Equal(3, result.Count);
        Assert.True(result[0].Equals(data1));
        Assert.True(result[1].Equals(data2));
        Assert.True(result[2].Equals(data3));
        
        fixture.Dispose();
    }
    
    [Fact]
    public void Integration_ExsistingFileReading2Newest_ShouldBeAbelToWriteAndRead()
    {   
        //Arrange
        fixture.SetupTestCsvDatabase();
        
        CsvTestObject data1 = new ("test1", 1, DateTime.Now, true);
        CsvTestObject data2 = new ("test2", 2, DateTime.Now.AddHours(-1234), false);
        CsvTestObject data3 = new ("test3", 3, DateTime.Now.AddHours(1234), true);

        List<CsvTestObject> result = new ();
        
        //Act
        fixture.csvDatabase.Store(data1);
        fixture.csvDatabase.Store(data2);
        fixture.csvDatabase.Store(data3);

        result = fixture.csvDatabase.Read(2).ToList();

        //Asert
        Assert.Equal(2, result.Count);
        Assert.True(result[0].Equals(data2));
        Assert.True(result[1].Equals(data3));
        
        fixture.Dispose();
    }
}