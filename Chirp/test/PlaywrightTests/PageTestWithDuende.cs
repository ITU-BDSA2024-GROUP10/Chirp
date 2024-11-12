using CsvHelper;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;


namespace PlaywrightTests;

public class PageTestWithDuende : PlaywrightTest
{
    protected const string DuendeBaseUrl = "http://localhost:5001";
    protected const string RazorBaseUrl = "http://localhost:5273";

    //private IPlaywright _playwright;
    private IBrowser _browser;
    protected IPage Page { get; private set; }

    private DuendePlaywrightWebAppFactory _duendeFactory;
    private CustomWebApplicationFactory _razorFactory;

    private HttpClient _duendeClient;
    private HttpClient _razorClient;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _duendeFactory = new DuendePlaywrightWebAppFactory(DuendeBaseUrl);
        _razorFactory = new CustomWebApplicationFactory(RazorBaseUrl);
        
        //_playwright = await Playwright.CreateAsync();
        
        //_browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        
    }

    [SetUp]
    public async Task Setup()
    {
        //for pop with bowser that does the steps, set headless to false, useful for debugging
        _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        
        var Context = await _browser.NewContextAsync();
        Page = await Context.NewPageAsync();
        
        _duendeClient = _duendeFactory.CreateClient();
        _razorClient = _razorFactory.WithWebHostBuilder(builder => builder.UseUrls(RazorBaseUrl)).CreateClient();
    }

    [TearDown]
    public async Task Teardown()
    {
        _duendeClient.Dispose();
        _razorClient.Dispose();
        await Page.CloseAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _duendeClient.Dispose();
        _razorClient.Dispose();
        await _duendeFactory.DisposeAsync();
        await _razorFactory.DisposeAsync();
        await _browser.CloseAsync();
    }
}