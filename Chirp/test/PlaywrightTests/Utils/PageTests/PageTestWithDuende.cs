using Microsoft.Playwright;
using PlaywrightTests.Utils.Factories;

namespace PlaywrightTests.Utils.PageTests;

public class PageTestWithDuende : PageTestWithRazorPlaywrightWebApplicationFactory
{
    protected const string DuendeBaseUrl = "http://localhost:5001";

    private DuendePlaywrightWebAppFactory _duendeFactory;

    private HttpClient _duendeClient;
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            Locale = "en-US",
            ColorScheme = ColorScheme.Light
        };
    }
    
    [OneTimeSetUp]
    public void DuendeOneTimeSetUp()
    {
        _duendeFactory = new DuendePlaywrightWebAppFactory(DuendeBaseUrl);
    }

    [SetUp]
    public void DuendeSetup()
    {
        _duendeClient = _duendeFactory.CreateClient();
    }

    [TearDown]
    public async Task DuendeTeardown()
    {
        _duendeClient.Dispose();
        await Page.CloseAsync();
    }

    [OneTimeTearDown]
    public async Task DuendeOneTimeTearDown()
    {
        _duendeClient.Dispose();
        await _duendeFactory.DisposeAsync();
    }
}