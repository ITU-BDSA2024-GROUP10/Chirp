using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.Utils.Factories;

namespace PlaywrightTests.Utils.PageTests;

public class PageTestWithRazorPlaywrightWebApplicationFactory : PageTest
{
    protected const string RazorBaseUrl = "http://localhost:5273";
    protected RazorPlaywrightWebApplicationFactory RazorFactory;
    private HttpClient _razorClient;
    protected PageUtils RazorPageUtils;

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
    public void RazorOneTimeSetUp() => RazorFactory = new RazorPlaywrightWebApplicationFactory(RazorBaseUrl);

    [SetUp]
    public void RazorSetup()
    {
        _razorClient = RazorFactory.CreateClient();
        // Note this does not make a new database it just wipes the data 
        RazorFactory.ResetDB();
        RazorPageUtils = new PageUtils(Page);
    }
    
    [TearDown]
    public void TearDown() => _razorClient.Dispose();

    [OneTimeTearDown]
    public async Task RazorOneTimeTearDown()
    {
        _razorClient.Dispose();
        await RazorFactory.DisposeAsync();
    }


    protected async Task<Cheep> GenerateCheep(Author author, string message = "test")
    {
        return (await GenerateCheeps(author, 1, message)).First();
    }
    
    protected async Task<IEnumerable<Cheep>> GenerateCheeps(Author author, int count, string message = "test")
    {
        var context = RazorFactory.GetDbContext();
        // This is to make sure the context now the user exists, since we add user through the user manager.
        // Otherwise, it will try to add the user again, and it will throw an exception.
        if (context.Authors.Local.All(a => a.Id != author.Id))
        {
            context.Authors.Attach(author);
        }
        
        var cheeps = new List<Cheep>();
        for (var i = 0; i < count; i++)
        {
            cheeps.Add(new Cheep()
            {
                Author = author,
                Message = message,
                TimeStamp = DateTime.Now.AddHours(i),
            });
        }
        
        await context.Cheeps.AddRangeAsync(cheeps);
        await context.SaveChangesAsync();
        return cheeps;
    }
}