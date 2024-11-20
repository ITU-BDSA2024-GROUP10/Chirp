using System.Text.RegularExpressions;
using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;
using System;
using System.Threading.Tasks;


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
        await Page.GotoAsync("http://localhost:5273/?page=1");

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