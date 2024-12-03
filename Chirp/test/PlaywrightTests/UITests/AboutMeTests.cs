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
        var testAuthor1 = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("mr. test1")
            .GetTestAuthor();
        var testAuthor2 = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("mr. test2")
            .GetTestAuthor();
        
        var testAuthorMain = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("mr. testMain")
            .GetTestAuthor();
        
        testAuthorMain.AddFollow(testAuthor1);
        testAuthorMain.AddFollow(testAuthor2);
        testAuthor1.AddFollow(testAuthorMain);
        
        testAuthorMain.Create();
        
        await GenerateCheeps(testAuthorMain.author, 365);
        
        await RazorPageUtils.Login(testAuthorMain);
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"Name: {testAuthorMain.UserName}" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"Email: {testAuthorMain.Email}" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "You have Cheep'd: 365 times" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Following: 2" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Followers: 1" })).ToBeVisibleAsync();
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

    [Test]
    public async Task CanUploadProfilePicture()
    {  
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await RazorPageUtils.Login(testAuthor);
        await Page.GotoAsync($"/AboutMe");
        await Page.GetByLabel("Upload Profile Image").ClickAsync();
        await Page.GetByLabel("Upload Profile Image").SetInputFilesAsync(new[] { "../../../Utils/testImages/test-profile-image.png" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Upload" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Profile Image" })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task UploadProfilePictureShowsErrorOnInvalidFile()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await RazorPageUtils.Login(testAuthor);
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Upload" }).ClickAsync();
        await Expect(Page.GetByText("Please upload an image.")).ToBeVisibleAsync();
    }
}