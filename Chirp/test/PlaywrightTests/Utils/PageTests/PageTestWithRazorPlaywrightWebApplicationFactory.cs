using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.Utils.Factories;

namespace PlaywrightTests.Utils.PageTests;

public class PageTestWithRazorPlaywrightWebApplicationFactory : PageTest
{
    protected const string RazorBaseUrl = "http://localhost:5273";
    protected RazorPlaywrightWebApplicationFactory razorFactory;
    private HttpClient _razorClient;

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
    public void RazorOneTimeSetUp() => razorFactory = new RazorPlaywrightWebApplicationFactory(RazorBaseUrl);

    [SetUp]
    public void RazorSetup()
    {
        _razorClient = razorFactory.CreateClient();
        razorFactory.ResetDB();
    }
    
    [TearDown]
    public void TearDown() => _razorClient.Dispose();

    [OneTimeTearDown]
    public async Task RazorOneTimeTearDown()
    {
        _razorClient.Dispose();
        await razorFactory.DisposeAsync();
    }
}