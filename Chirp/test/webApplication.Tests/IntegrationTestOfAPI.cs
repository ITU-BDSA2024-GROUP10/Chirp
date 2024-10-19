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
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }*/
}