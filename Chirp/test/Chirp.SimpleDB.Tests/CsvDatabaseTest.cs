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
    private readonly List<CsvTestObject> testData;

    public CsvDatabaseTest()
    {
        fixture = new CsvDatabaseTextFixture();
        
        CsvTestObject data1 = new ("test1", 1, DateTime.Now, true);
        CsvTestObject data2 = new ("test2", 2, DateTime.Now.AddHours(-1234), false);
        CsvTestObject data3 = new ("test3", 3, DateTime.Now.AddHours(1234), true);
        
        testData = new List<CsvTestObject> {data1, data2, data3};
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
    public void STORE_ExsistingFile_ShouldWriteRecords()
    {
        //Arrange
        fixture.CreateTestCsvDatabaseFile();
        List<CsvTestObject> result = new ();
        
        //Act
        foreach (var data in testData)
        {
            fixture.csvDatabase.Store(data);
        }
        result = fixture.ExtractDataFromCsvDatabase();
        
        //Asert
        Assert.Equal(3, result.Count);
        Assert.True(fixture.HasHeader());
        Assert.Equal(testData[0], result[0]);
        Assert.Equal(testData[1], result[1]);
        Assert.Equal(testData[2], result[2]);
        
        fixture.Dispose();
    }
    
    [Fact]
    public void STORE_NonExsistingFile_ShouldWriteRecordsAndHeader()
    {
        //Arrange
        List<CsvTestObject> result = new ();
        
        //Act
        foreach (var data in testData)
        {
            fixture.csvDatabase.Store(data);
        }
        result = fixture.ExtractDataFromCsvDatabase();
        
        //Asert
        Assert.Equal(3, result.Count);
        Assert.True(fixture.HasHeader());
        Assert.Equal(testData[0], result[0]);
        Assert.Equal(testData[1], result[1]);
        Assert.Equal(testData[2], result[2]);
        
        fixture.Dispose();
    }
    
    [Fact]
    public void READ_AllRecords_ShouldReadAllRecords()
    {
        //Arrange
        fixture.CreateTestCsvDatabaseFile();
        fixture.WriteTestDataToCsvDatabase(testData);
        List<CsvTestObject> result = new ();
        
        //Act
        result = fixture.csvDatabase.Read(3).ToList();
        
        //Asert
        Assert.Equal(3, result.Count);
        Assert.Equal(testData[0], result[0]);
        Assert.Equal(testData[1], result[1]);
        Assert.Equal(testData[2], result[2]);
        
        fixture.Dispose();
    }
    
    [Fact]
    public void READ_LimitHigherThanNumberOfRecors_ShouldReadAllRecords()
    {
        //Arrange
        fixture.CreateTestCsvDatabaseFile();
        fixture.WriteTestDataToCsvDatabase(testData);
        List<CsvTestObject> result = new ();
        
        //Act
        result = fixture.csvDatabase.Read(10).ToList();
        
        //Asert
        Assert.Equal(3, result.Count);
        Assert.Equal(testData[0], result[0]);
        Assert.Equal(testData[1], result[1]);
        Assert.Equal(testData[2], result[2]);
        
        fixture.Dispose();
    }
    
    [Fact]
    public void READ_LimitLowerThanNumberOfRecors_ShouldReadLimitNewestRecords()
    {
        //Arrange
        fixture.CreateTestCsvDatabaseFile();
        fixture.WriteTestDataToCsvDatabase(testData);
        List<CsvTestObject> result = new ();
        
        //Act
        result = fixture.csvDatabase.Read(2).ToList();
        
        //Asert
        Assert.Equal(2, result.Count);
        Assert.Equal(testData[1], result[0]);
        Assert.Equal(testData[2], result[1]);
    }

        
        fixture.Dispose();
    }
}