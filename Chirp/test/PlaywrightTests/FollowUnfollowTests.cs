using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;


namespace PlaywrightTests;

//[Parallelizable(ParallelScope.Self)]
[TestFixture]
[NonParallelizable]
public class FollowUnfollowTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [SetUp]
    public async Task SetUp()
    {
        var author = new Author
        {
            UserName = "Mathias",
            Email = "mlao@itu.dk"
        };
        var password = "Password123!";
        
        // register author
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(author.UserName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(author.Email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync(); 

        // login author
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(author.UserName);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // cheep test chirp
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("test chirp :)");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        // logout author
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [testuser]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        var follower = new Author
        {
            UserName = "Follower",
            Email = "follower@itu.dk"
        };
        
        // register
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(follower.UserName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(follower.Email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync(); 

        // login
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(follower.UserName);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }
    
    [Test]
    public async Task UserCanFollowAuthor()
    {
        
    }

    [Test]
    public async Task UserCanUnfollowAuthor()
    {
       
    }

    [Test]
    public async Task UserCannotFollowSelf()
    {
       
    }

    [Test]
    public async Task CanOnlySeeOwnAndFollowedCheeps()
    {
       
    }

    [Test]
    public async Task UserStaysOnPageAfterUnfollowOnPrivateTimeline()
    {
       
    }

    [Test]
    public async Task UserStaysOnPageAfterFollowAndUnfollowOnPublicTimeline()
    {
       
    }
}