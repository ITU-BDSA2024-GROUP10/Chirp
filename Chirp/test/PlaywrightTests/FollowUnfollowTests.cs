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
        var context = razorFactory.GetDbContext();

        // add author user
        Author testAuthor = new()
        {
            UserName = "mr. author",
            Email = "author@test.com"
        };
        context.Authors.Add(testAuthor);

        // cheep test cheep
        context.Cheeps.Add(new Cheep()
        {
            Author = testAuthor,
            Message = "test",
        });

        // register and login mr. follower
        var follower = new Author
        {
            UserName = "Follower",
            Email = "follower@itu.dk"
        };
        var password = "Password123!";

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
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync(); 

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
        await Page.GetByText("mr. author follow test").ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "mr. author follow test" }).GetByRole(AriaRole.Button).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.GetByText("mr. author  unfollow test")).ToBeVisibleAsync();
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