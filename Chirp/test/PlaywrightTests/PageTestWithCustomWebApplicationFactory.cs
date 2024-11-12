using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

public class PageTestWithCustomWebApplicationFactory : PageTest
{
    private const string RazorBaseUrl = "http://localhost:5273";
    private CustomWebApplicationFactory _factory;
    private HttpClient _client;

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            Locale = "en-US",
            ColorScheme = ColorScheme.Light,
            BaseURL = RazorBaseUrl,
        };
    }

    [OneTimeSetUp]
    public void OneTimeSetUp() => _factory = new CustomWebApplicationFactory(RazorBaseUrl);

    [SetUp]
    public void Setup()
    {
        _client = _factory.WithWebHostBuilder(builder => builder.UseUrls(RazorBaseUrl)).CreateClient();
        _factory.ResetDB();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }
}