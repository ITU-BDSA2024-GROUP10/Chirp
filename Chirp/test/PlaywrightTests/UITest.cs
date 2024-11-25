using System.Text.RegularExpressions;
using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;
using TestUtilities;

namespace PlaywrightTests;

[NonParallelizable]
public class UITest : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [Test]
    public async Task HasTitle()
    {
        //act
        await Page.GotoAsync("/");

        //assert
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
    }

    [Test]
    public async Task HasRegisterLink()
    {
        //act
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();

        //assert
        await Expect(Page).ToHaveTitleAsync(new Regex("Register"));
    }

    [Test]
    public async Task PasswordMustHaveNonAlphanumericCharacters()
    {
        //act
        await Page.GotoAsync("/Identity/Account/Register");
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("name");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password123");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        //assert
        await Expect(Page.GetByText("Passwords must have at least")).ToBeVisibleAsync();
    }

    [Test]
    [TestCase("?page=1", "-5")]
    [TestCase("?page=1", "0")]
    [TestCase("?page=2", "2")]
    [TestCase("?page=1", null)]
    public async Task PaginationOfPublicTimeline(string expectedEndpoint, string page)
    {
        //act
        await Page.GotoAsync($"/?page={page}");

        //assert
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}/{expectedEndpoint}");
    }

    [Test]
    [TestCase("?page=1", "-5")]
    [TestCase("?page=1", "0")]
    [TestCase("?page=2", "2")]
    [TestCase("?page=1", null)]
    public async Task PaginationOfUserTimeline(string expectedEndpoint, string page)
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("Mr.test")
            .Create();
        
        //act
        await Page.GotoAsync($"/{testAuthor.UserName!.ToLower()}?page={page}");

        //assert
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}/{testAuthor.UserName.ToLower()}{expectedEndpoint}");
    }

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
    public async Task SeeCorrectNumberOfCheepsOnPublicTimeline()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.author, 33);

        //act
        await Page.GotoAsync("/");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(32);
        await Page.GotoAsync("/?page=2");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(1);
        await Page.GotoAsync("/?page=3");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(0);
    }

    [Test]
    public async Task SeeCorrectNumberOfCheepsOnPrivateTimeline()
    {
        #region Arrange
        var realTestAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("Mr. real")
            .Create();
        var fakeTestAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("Mr. fake")
            .Create();
        await GenerateCheeps(realTestAuthor.author, 33);
        await GenerateCheeps(fakeTestAuthor.author, 33);

        #endregion

        //act
        await Page.GotoAsync($"/{realTestAuthor.UserName}");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(32);
        await Page.GotoAsync($"/{realTestAuthor.UserName}?page=2");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(1);
        await Page.GotoAsync($"/{realTestAuthor.UserName}?page=3");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(0);
    }

    [Test]
    public async Task CheepBoxNotVisibleWhileLoggedOut()
    {
        await Page.GotoAsync("/");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Log in to post Cheeps!" }))
            .ToBeVisibleAsync();
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
    public async Task CheepBoxVisibleWhileLoggedIn()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        
        await RazorPageUtils.Login(testAuthor);
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"What's on your mind {testAuthor.UserName}?" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#Message")).ToBeVisibleAsync();
    }

    [Test]
    public async Task PageButtons_FirstMiddleEndPages()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.author, 65);
        
        //first
        await Page.GotoAsync("/");
        await Expect(Page.Locator("body")).ToContainTextAsync("1 Next");
        //middle
        await Page.GetByRole(AriaRole.Link, new() { Name = "Next" }).First.ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("Previous 2 Next");
        //end
        await Page.GotoAsync("/?page=9999");
        await Expect(Page.Locator("body")).ToContainTextAsync("Previous 9999");
    }

    [Test]
    public async Task ThePrivateTimeLineIsCaseInSensitive()
    {
        #region Arrange
        
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("MR. tESt")
            .Create();
        var cheep = await GenerateCheep(testAuthor.author);
        
        #endregion

        //act
        await Page.GotoAsync($"/{testAuthor.UserName!.ToUpper()}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
        
        await Page.GotoAsync($"/{testAuthor.UserName.ToLower()}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
        
        await Page.GotoAsync($"/{testAuthor.UserName}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
    }

    [Test]
    public async Task PageNotFound_ShownWhenUserNotFound()
    {
        await Page.GotoAsync($"/ugsrniutgfnbdfikjfns");
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}/notfound");
    }
}