using Duende.IdentityServer.Extensions;
using Microsoft.Playwright;
using NUnit.Framework.Internal;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests;

[TestFixture]
[NonParallelizable]
public class FollowUnfollowTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    private TestAuthor _testAuthor;
    private TestAuthor _testFollower;
    
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
        _testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("author")
            .Create();
        await GenerateCheep(_testAuthor.author);
        
        _testFollower = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("follower")
            .Create();
    }

    [Test]
    public async Task CantSeeFollowButtonWhenNotLoggedIn()
    {
        var followButton = Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button);
        await Expect(followButton).ToBeHiddenAsync();
    }

    [Test]
    public async Task CanSeeFollowButtonWhenLoggedIn()
    {
        await RazorPageUtils.Login(_testFollower);
        var followButton = Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button);
        await Expect(followButton).ToBeVisibleAsync();
    }
        
    
    [Test]
    public async Task UserCanFollowAuthor()
    {
        await RazorPageUtils.Login(_testFollower);
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await Expect(Page.GetByText("author unfollow test")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCanUnfollowAuthor()
    {
        await RazorPageUtils.Login(_testFollower);
        
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await UnfollowAuthor("author unfollow test");
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCannotFollowSelf()
    {
        await RazorPageUtils.Login(_testAuthor);
        var followButton = Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button);
        await Expect(followButton).ToBeHiddenAsync();
    }

    [Test]
    public async Task CanOnlySeeOwnAndFollowedCheeps()
    {
        var testAuthor2 = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("author2")
            .Create();
        await GenerateCheep(testAuthor2.author, "this is author2's cheep");
        await RazorPageUtils.Login(_testFollower);
        
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await Expect(Page.Locator("li").Filter(new() { HasText = "author2 follow this is author2's cheep" })).ToBeHiddenAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "author unfollow test" })).ToBeVisibleAsync();
    }
    [Test]
    public async Task ForgetMeLogsOutUserAndRemovesData()
    {
        await GenerateCheep(_testAuthor.author, "this is author");
        await GenerateCheep(_testFollower.author, "this is follower");
        await RazorPageUtils.Login(_testFollower);
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await RazorPageUtils.Logout(_testFollower.UserName!);
        await Page.GotoAsync("/");
        await RazorPageUtils.Login(_testAuthor);
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Yes, Delete" }).ClickAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "this is author" })).ToBeHiddenAsync();
        Assert.That(_testFollower.Follows.IsNullOrEmpty());


    }

    [Test]
    public async Task UserStaysOnPageAfterUnfollowOnPrivateTimeline()
    {
        await RazorPageUtils.Login(_testFollower);
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        await GoToPrivateTimeline();
        await UnfollowAuthor("author unfollow test");
        Assert.That(Page.Url, Is.EqualTo($"{RazorBaseUrl}/{_testFollower.UserName!.ToLower()}?page=1"));
    }

    [Test]
    public async Task UserStaysOnPageAfterFollowAndUnfollowOnPublicTimeline()
    {
        await RazorPageUtils.Login(_testFollower);
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");
        Assert.That(Page.Url, Is.EqualTo($"{RazorBaseUrl}/?page=1"));
        await UnfollowAuthor("author unfollow test");
        Assert.That(Page.Url, Is.EqualTo($"{RazorBaseUrl}/?page=1"));
    }

    [Test]
    public async Task FollowUnfollowMaintainsScrollPosition()
    {
        await RazorPageUtils.Login(_testFollower);
        await GoToPublicTimeline();
        await Page.EvaluateAsync("() => window.scrollTo(0, 500)");
        var initialScrollPosition = await Page.EvaluateAsync<int>("() => window.scrollY");
        await FollowAuthor("author follow test");

        // assert scroll position unchanged
        var scrollPositionAfterFollow = await Page.EvaluateAsync<int>("() => window.scrollY");
        Assert.That(scrollPositionAfterFollow, Is.EqualTo(initialScrollPosition));
        await UnfollowAuthor("author unfollow test");
        var scrollPositionAfterUnfollow = await Page.EvaluateAsync<int>("() => window.scrollY");
        Assert.That(scrollPositionAfterUnfollow, Is.EqualTo(initialScrollPosition));
    }

    [Test]
    public async Task FollowUnfollowUpdatesButtonCorrectly()
    {
        await RazorPageUtils.Login(_testFollower);
        await GoToPublicTimeline();
        await FollowAuthor("author follow test");

        // assert follow button changed to unfollow
        var unfollowButton = Page.Locator("li").Filter(new() { HasText = "author unfollow test" }).GetByRole(AriaRole.Button);
        await Expect(unfollowButton).ToBeVisibleAsync();

        await UnfollowAuthor("author unfollow test");

        // assert unfollow button changed back
        var followButton = Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button);
        await Expect(followButton).ToBeVisibleAsync();
    }
}