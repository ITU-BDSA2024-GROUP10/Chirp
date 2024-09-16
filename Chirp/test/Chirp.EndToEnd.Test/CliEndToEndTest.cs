using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using SimpleDB;
using Xunit;

namespace Chirp.EndToEnd.Test;

[TestSubject(typeof(Cheep))]
public class CliEndToEndTest : IDisposable
{
    string filePath = "end2endtest.csv";
    IDatabaseRepository<Cheep> dbr;

    public CliEndToEndTest()
    {
        dbr = new CsvDatabase<Cheep>(filePath, new CheepMap());
    }
    

    [Fact]
    public void TestRead()
    {
        ArrangeDataBase();
        string end = "";

        using (var process = new Process())
        {
            process.StartInfo.FileName = "/usr/bin/dotnet";
            process.StartInfo.Arguments = "/Chirp/src/Chirp.CLI.Client/bin/Debug/net7.0/Chirp.CLI.Client.dll read 10";
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