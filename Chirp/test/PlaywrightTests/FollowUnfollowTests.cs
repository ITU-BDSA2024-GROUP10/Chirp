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
        // navigate to the page
        await Page.GotoAsync("http://localhost:5273/");

        // login
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testuser");
        await Page.GetByPlaceholder("Username").PressAsync("Tab");
        await Page.GetByPlaceholder("password").FillAsync("Test1234!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // assert login success by checking present of my timeline
        var myTimelineLink = Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" });
        await Assertions.Expect(myTimelineLink).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task UserCanFollowAuthor()
    {
        // follow Jacqualine
        var followButton = Page.Locator("li")
            .Filter(new() { HasText = "Jacqualine Gilcoine follow Starbuck now is what we hear the worst. — 01/08/23" })
            .GetByRole(AriaRole.Button);

        await Assertions.Expect(followButton).ToBeVisibleAsync(); // Assert the button is visible
        await followButton.ClickAsync();

        // navigate to private timeline
        var myTimelineLink = Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" });
        await myTimelineLink.ClickAsync();

        // assert Jacqualine is on the private timeline
        var jacqualinePost = Page.Locator("li")
            .Filter(new() { HasText = "Jacqualine Gilcoine unfollow Starbuck now is what we hear the worst. — 01/08/23" })
            .GetByRole(AriaRole.Paragraph).First;

        await Assertions.Expect(jacqualinePost).ToBeVisibleAsync(); // assert the post is visible

        // unfollow
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine unfollow Starbuck now is what we hear the worst. — 01/08/23" }).GetByRole(AriaRole.Button).ClickAsync();
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