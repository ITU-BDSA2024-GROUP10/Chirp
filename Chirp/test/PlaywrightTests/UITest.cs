using System.Text.RegularExpressions;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests;

[NonParallelizable]
public class UITest : PageTestWithCustomWebApplicationFactory
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
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}{expectedEndpoint}");
    }

    [Test]
    [TestCase("test?page=1", "-5")]
    [TestCase("test?page=1", "0")]
    [TestCase("test?page=2", "2")]
    [TestCase("test?page=1", null)]
    public async Task PaginationOfUserTimeline(string expectedEndpoint, string page)
    {
        //act
        await Page.GotoAsync($"/test?page={page}");

        //assert
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}{expectedEndpoint}");
    }

    [Test]
    public async Task SeeCorrectNumberOfCheepsOnPublicTimeline()
    {
        //arrange
        var context = razorFactory.GetDbContext();
        Author testAuthor = new Author
        {
            Name = "mr. test",
            Email = "test@test.com"
        };
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
        Author realTestAuthor = new Author
        {
            Name = "mr. test",
            Email = "realtest@test.com"
        };
        Author fakeTestAuthor = new Author
        {
            Name = "fake mr. test",
            Email = "faketest@test.com"
        };
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
        await Page.GotoAsync($"/{realTestAuthor.Name}");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(32);
        await Page.GotoAsync($"/{realTestAuthor.Name}?page=2");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(1);
        await Page.GotoAsync($"/{realTestAuthor.Name}?page=3");
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
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("Mathias");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("mlao@itu.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("mlao@itu.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind Mathias?" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#Message")).ToBeVisibleAsync();
    }
}