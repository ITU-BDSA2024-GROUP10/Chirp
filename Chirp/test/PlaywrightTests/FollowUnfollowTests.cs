using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests;

[TestFixture]
[NonParallelizable]
public class FollowUnfollowTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    private const string DefaultPassword = "Password123!";

    // helper method: register a user
    private async Task RegisterUser(string userName, string email, string password)
    {
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(userName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync();
    }

    // helper method: log in as a user
    private async Task LoginAsUser(string userName, string password)
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(userName);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }

    // helper method for log out
    private async Task Logout()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }

    // helper method for write a cheep
    private async Task WriteCheep(string message)
    {
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync(message);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    }

    // helper method for following author
    private async Task FollowAuthor(string authorCheepText)
    {
        await Page.Locator("li").Filter(new() { HasText = authorCheepText }).GetByRole(AriaRole.Button).ClickAsync();
    }

    // helper method for unfollow an author
    private async Task UnfollowAuthor(string authorCheepText)
    {
        await Page.Locator("li").Filter(new() { HasText = authorCheepText }).GetByRole(AriaRole.Button).ClickAsync();
    }

    // helper method for navigating to public timeline
    private async Task GoToPublicTimeline()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
    }

    // helper method fpr navigating to private timeline
    private async Task GoToPrivateTimeline()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        await RegisterUser("author", "author@itu.dk", DefaultPassword);
        await LoginAsUser("author", DefaultPassword);
        await WriteCheep("test");
        await Logout();

        await RegisterUser("follower", "follower@itu.dk", DefaultPassword);
        await LoginAsUser("follower", DefaultPassword);
    }

    [Test]
    public async Task UserCanFollowAuthor()
    {
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await Expect(Page.GetByText("author unfollow test")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCanUnfollowAuthor()
    {
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await UnfollowAuthor("author unfollow test");
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCannotFollowSelf()
    {
        await Logout();
        await LoginAsUser("author", DefaultPassword);
        var followButton = Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button);
        await Expect(followButton).ToBeHiddenAsync();
    }

    [Test]
    public async Task CanOnlySeeOwnAndFollowedCheeps()
    {
        await Logout();
        await RegisterUser("author2", "author2@itu.dk", DefaultPassword);
        await LoginAsUser("author2", DefaultPassword);
        await WriteCheep("this is author2's cheep");
        await Logout();
        await LoginAsUser("follower", DefaultPassword);
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await Expect(Page.Locator("li").Filter(new() { HasText = "this is author2's cheep" })).ToBeHiddenAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "author unfollow test" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserStaysOnPageAfterUnfollowOnPrivateTimeline()
    {
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await UnfollowAuthor("author unfollow test");
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserStaysOnPageAfterFollowAndUnfollowOnPublicTimeline()
    {
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await UnfollowAuthor("author unfollow test");
        await Expect(Page.Locator("li").Filter(new() { HasText = "author follow test" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeHiddenAsync();
    }
}