using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using SimpleDB;
using Xunit;
using Xunit.Abstractions;

namespace Chirp.EndToEnd.Test;

[TestSubject(typeof(Cheep))]
public class CliEndToEndTest : IDisposable
{
    string filePath = "end2endtest.csv";
    IDatabaseRepository<Cheep> dbr;
    private readonly ITestOutputHelper testPrint;
    public CliEndToEndTest(ITestOutputHelper testPrint)
    {
        dbr = new CsvDatabase<Cheep>(filePath, new CheepMap());
        this.testPrint = testPrint;
    }

    [Fact]
    public void TestRead()
    {
        ArrangeDataBase();
        string end = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "run --project src\\Chirp.CLI.Client read 10";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../";
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            
            StreamReader reader = process.StandardOutput;
            end = reader.ReadToEnd();
            process.WaitForExit();
        }

        string firstLine = end.Split("\n")[0];
        Assert.Equal("tester,\"Hello, World!, test\",09/12/2024 12:21:15", firstLine);

    }
    
    
    [Fact]
    public void TestCheep()
    {
        Assert.True(true);
    }


    void ArrangeDataBase()
    {
        if (File.Exists(filePath))
        {
            DisposeTestdata();
        }
        using StreamWriter sw = new StreamWriter(filePath);
        sw.WriteLine("Author,Message,Date");
        sw.WriteLine("tester,\"Hello, World!, test\",09/12/2024 12:21:15");
        
    }

    void DisposeTestdata()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void Dispose()
    {
        DisposeTestdata();
    }
}