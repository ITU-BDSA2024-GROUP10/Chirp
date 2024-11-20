using System.Globalization;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Chirp.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using TestUtilities;

namespace webApplication.Tests;

public class TestAPI : IClassFixture<InMemoryCostumeWebApplicationFactory>
{
    private readonly InMemoryCostumeWebApplicationFactory fixture;
    private readonly HttpClient client;

    public TestAPI(InMemoryCostumeWebApplicationFactory fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(new WebApplicationFactoryClientOptions
            { AllowAutoRedirect = true, HandleCookies = true });
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
    public async Task CanSeePrivateTimeline(string author)
    {
        //arrange
        fixture.ResetDB();

        var wantedAuthor = new Author { UserName = author, Email = $"{author}@gmail.com" };
        var otherAuthor = new Author { UserName = "Mr. test", Email = "test@test.com" };
        wantedAuthor.NormalizedUserName = wantedAuthor.UserName.ToUpper();
        otherAuthor.NormalizedUserName = otherAuthor.UserName.ToUpper();
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
            await context.SaveChangesAsync();
        }

        //act
        var response = await client.GetAsync($"/{author.ToLower()}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        //assert
        Assert.Contains(wantedAuthor.UserName, content);
        Assert.Contains(wantedCheep.Message, content);
        Assert.Contains(wantedCheep.TimeStamp.ToUniversalTime().ToString("dd/MM/yy H:mm:ss"), content);

        Assert.DoesNotContain(otherAuthor.UserName, content);
        Assert.DoesNotContain(otherCheep.Message, content);
        Assert.DoesNotContain(otherCheep.TimeStamp.ToUniversalTime().ToString("dd/MM/yy H:mm:ss"), content);

        Assert.Contains($"{wantedAuthor.UserName}'s Timeline", content);
    }

    [Fact]
    public async Task CanSeePublicTimeline()
    {
        fixture.ResetDB();
        var wantedAuthor = new Author { UserName = "Wanted", Email = "wanted@gmail.com" };
        var wantedCheep = new Cheep
        {
            Author = wantedAuthor,
            Message = "This a test, from the real author!",
            TimeStamp = DateTime.Now
        };
        await using (var context = fixture.GetDbContext())
        {
            context.Authors.Add(wantedAuthor);
            context.Cheeps.Add(wantedCheep);
            await context.SaveChangesAsync();
        }

        var response = await client.GetAsync($"/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains(wantedAuthor.UserName, content);
        Assert.Contains(wantedCheep.Message, content);
        Assert.Contains(wantedCheep.TimeStamp.ToUniversalTime().ToString("dd/MM/yy H:mm:ss"), content);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task PaginationChangesDisplayedCheepsTest(int page)
    {
        fixture.ResetDB();
        var cheepslist = new List<Cheep>();
        var author = new Author
        {
            UserName = "Sir Page",
            Email = "Page1@gmail.com",
        };

        for (int i = 1; i <= 64; i++)
        {
            cheepslist.Add(new Cheep
            {
                Author = author,
                Message = "Message " + i + " Cheep cheep",
                TimeStamp = DateTime.Now.AddHours(i)
            });
        }

        await using (var context = fixture.GetDbContext())
        {
            context.Authors.Add(author);
            context.Cheeps.AddRange(cheepslist);

            await context.SaveChangesAsync();
        }

        var response = await client.GetAsync($"/?page={page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        cheepslist.Reverse();
        var expectedCheeps = cheepslist.Skip((page - 1) * 32).Take(32).ToList();
        cheepslist.RemoveAll(c => expectedCheeps.Contains(c));

        foreach (var cheep in expectedCheeps)
        {
            Assert.Contains(cheep.Message, content);
            Assert.Contains(cheep.TimeStamp.ToUniversalTime().ToString("dd/MM/yy H:mm:ss"), content);
            Assert.Contains(cheep.Author.UserName!, content);
        }

        foreach (var cheep in cheepslist)
        {
            Assert.DoesNotContain(cheep.Message, content);
            Assert.DoesNotContain(cheep.TimeStamp.ToUniversalTime().ToString("dd/MM/yy H:mm:ss"), content);
        }
    }
}