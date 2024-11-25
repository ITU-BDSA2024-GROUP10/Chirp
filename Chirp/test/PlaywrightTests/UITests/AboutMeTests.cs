using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

[NonParallelizable]
public class AboutMeTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [Test]
    public async Task CanSeeAboutMeLinkWhenRegistered()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        
        await RazorPageUtils.Login(testAuthor);
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "About Me" })).ToBeVisibleAsync();
        
    }

    [Test]
    public async Task CannotSeeAboutMeLinkWhenNotRegistered()
    {
        await Page.GotoAsync($"/?page=1");
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "About Me" })).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task AboutMePageShowsUserInfo()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        
        await RazorPageUtils.Login(testAuthor);
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"Name: {testAuthor.UserName}" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"Email: {testAuthor.Email}" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "You have Cheep'd: 0 times" })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task ConfirmationPopupAppearsAndDisappears()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await RazorPageUtils.Login(testAuthor);
        await Page.GotoAsync($"/AboutMe");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();
        await Expect(Page.GetByText("Are you sure you want to delete your account? This action cannot be undone.")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();
        await Expect(Page.GetByText("Are you sure you want to delete your account? This action cannot be undone.")).ToBeHiddenAsync();
        
    }
}