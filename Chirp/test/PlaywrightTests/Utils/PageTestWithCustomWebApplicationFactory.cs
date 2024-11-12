using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

public class PageTestWithCustomWebApplicationFactory : PageTest
{
    protected const string RazorBaseUrl = "http://localhost:5273";
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
    public void RazorOneTimeSetUp() => _factory = new CustomWebApplicationFactory(RazorBaseUrl);

    [SetUp]
    public void RazorSetup()
    {
        _client = _factory.WithWebHostBuilder(builder => builder.UseUrls(RazorBaseUrl)).CreateClient();
        _factory.ResetDB();
    }

    [OneTimeTearDown]
    public async Task RazorOneTimeTearDown()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }
}