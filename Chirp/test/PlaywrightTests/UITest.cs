using System.Text.RegularExpressions;
using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;
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
        var context = razorFactory.GetDbContext();
        Author testAuthor = TestUtils.CreateTestAuthor("test");
        context.Authors.Add(testAuthor);
        await context.SaveChangesAsync();
        
        //act
        await Page.GotoAsync($"/{testAuthor.UserName!.ToLower()}?page={page}");

        //assert
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}/{testAuthor.UserName}{expectedEndpoint}");
    }

    [Test]
    public async Task CanSeeAboutMeLinkWhenRegistered()
    {
        
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
        
    }

    [Test]
    public async Task SeeCorrectNumberOfCheepsOnPublicTimeline()
    {
        //arrange
        var context = razorFactory.GetDbContext();
        Author testAuthor = TestUtils.CreateTestAuthor("Mr. test");
        context.Authors.Add(testAuthor);

        for (var i = 0; i < 33; i++)
        {
            context.Cheeps.Add(new Cheep()
            {
                Author = testAuthor,
                Message = "test",
                TimeStamp = DateTime.Now.AddHours(i),
            });
        }

        await context.SaveChangesAsync();

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

        var context = razorFactory.GetDbContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        Author realTestAuthor = TestUtils.CreateTestAuthor("Mr. test");
        Author fakeTestAuthor = TestUtils.CreateTestAuthor("Mr. fake");

        context.Authors.Add(realTestAuthor);
        context.Authors.Add(fakeTestAuthor);

        for (var i = 0; i < 33; i++)
        {
            context.Cheeps.Add(new Cheep()
            {
                Author = realTestAuthor,
                Message = "real test",
                TimeStamp = DateTime.Now.AddHours(i),
            });
        }

        for (var i = 0; i < 33; i++)
        {
            context.Cheeps.Add(new Cheep()
            {
                Author = fakeTestAuthor,
                Message = "fake test",
                TimeStamp = DateTime.Now.AddHours(i),
            });
        }

        await context.SaveChangesAsync();

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
    public async Task CheepBoxVisibleWhileLoggedIn()
    {
        var user = TestUtils.CreateTestAuthor("Mr. test");
        var password = "Password123!";

        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(user.UserName!);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(user.Email!);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(user.UserName!);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"What's on your mind {user.UserName}?" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#Message")).ToBeVisibleAsync();
    }

    [Test]
    public async Task PageButtons_FirstMiddleEndPages()
    {
        //arrange
        var context = razorFactory.GetDbContext();
        Author testAuthor = TestUtils.CreateTestAuthor("Mr. test");
        context.Authors.Add(testAuthor);

        for (var i = 0; i < 65; i++)
        {
            context.Cheeps.Add(new Cheep()
            {
                Author = testAuthor,
                Message = "test",
                TimeStamp = DateTime.Now.AddHours(i),
            });
        }

        await context.SaveChangesAsync();
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

        var context = razorFactory.GetDbContext();
        var author = TestUtils.CreateTestAuthor("MR. tESt");
        context.Authors.Add(author);

        var cheep = new Cheep
        {
            Author = author,
            Message = "test",
            TimeStamp = DateTime.Now
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        #endregion

        //act
        await Page.GotoAsync($"/{author.UserName!.ToUpper()}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
        
        await Page.GotoAsync($"/{author.UserName.ToLower()}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
        
        await Page.GotoAsync($"/{author.UserName}");
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