using System.Text.RegularExpressions;
using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

[NonParallelizable]
public class GeneralTests : PageTestWithRazorPlaywrightWebApplicationFactory
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
    public async Task CantCreateUserThatAlreadyExists()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(testAuthor.UserName!);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@test.test");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync($"Username '{testAuthor.UserName}' is already taken.");
    }

    [Test]
    public async Task CreateUserWithMissingInfo()
    {
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.Locator("#registerForm")).ToContainTextAsync("The Email field is required.");
        await Expect(Page.Locator("#registerForm")).ToContainTextAsync("The Password field is required.");
    }
    
    [Test]
    public async Task CantLoginUserThatDosentExists()
    {
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("testtest");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("Invalid login attempt.");
    }
}