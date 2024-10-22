using Chirp.Razor.DataModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SimpleDB;
using webApplication.Tests.Utils;

namespace webApplication.Tests;

public class TestAPI : IClassFixture<CostumeWebApplicationFactory<Program, ChirpDBContext>>
{
    private readonly CostumeWebApplicationFactory<Program, ChirpDBContext> fixture;
    private readonly HttpClient client;

    public TestAPI(CostumeWebApplicationFactory<Program, ChirpDBContext> fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    /*[Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }*/

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void CanSeePrivateTimeline(string author)
    {
        //arrange
        fixture.ResetDB();

        var wantedAuthor = new Author { Name = author, Email = $"{author}@gmail.com" };
        var otherAuthor = new Author { Name = "Mr. test", Email = "test@test.com" };
        var wantedCheep = new Cheep
        {
            Author = wantedAuthor,
            Message = "This a test, from the real author!",
            TimeStamp = DateTime.Now
        };
        var otherCheep = new Cheep
        {
            Author = otherAuthor,
            Message = "This a test, from the fake author!",
            TimeStamp = DateTime.Now.AddHours(1)
        };

        await using (var context = fixture.GetDbContext())
        {
            context.Authors.Add(wantedAuthor);
            context.Authors.Add(otherAuthor);
            context.Cheeps.Add(wantedCheep);
            context.Cheeps.Add(otherCheep);
            context.SaveChanges();
        }

        //act
        var response = await client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        //assert
        Assert.Contains(wantedAuthor.Name, content);
        Assert.Contains(wantedCheep.Message, content);
        Assert.Contains(wantedCheep.TimeStamp.ToUniversalTime().ToString(), content);

        Assert.DoesNotContain(otherAuthor.Name, content);
        Assert.DoesNotContain(otherCheep.Message, content);
        Assert.DoesNotContain(otherCheep.TimeStamp.ToUniversalTime().ToString(), content);

        Assert.Contains($"{wantedAuthor.Name}'s Timeline", content);
    }

    [Fact]
    public async void CanSeePublicTimeline()
    {
        
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void PaginationChangesDisplayedCheepsTest(int page)
    {
        fixture.ResetDB();
        var cheepslist = new List<Cheep>();
        var author = new Author
        {
            Name = "Sir Page",
            Email = "Page1@gmail.com",
        };

    }
}