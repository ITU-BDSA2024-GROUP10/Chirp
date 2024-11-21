using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;
using TestUtilities;

namespace PlaywrightTests;

public class SQLInjectionTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [Test]
    public async Task SQLInjectionInCheepTextBox()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
       
        await RazorPageUtils.Login(testAuthor);
        
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("105; DROP TABLE Cheeps;");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        
        //assert
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("105; DROP TABLE Cheeps;");
    }

    [Test]
    public async Task SQLInjectionInRegisterNameAndLoggingIn()
    {
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("DROP TABLE Cheeps;");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("cheep@gmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password!123");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password!123");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("Username 'DROP TABLE Cheeps;' is invalid, can only contain letters or digits.");
    }

    [Test]
    public async Task SQLInjectionInRegisterEmailAndLoggingIn()
    {
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("TestUser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("DROP TABLE Cheeps;@gmail.com");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password!123");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password!123");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByPlaceholder("name", new() { Exact = true })).ToHaveValueAsync("TestUser");
        await Expect(Page.GetByPlaceholder("name@example.com")).ToHaveValueAsync("DROP TABLE Cheeps;@gmail.com");
        await Expect(Page.GetByLabel("Password", new() { Exact = true })).ToHaveValueAsync("Password!123");
        await Expect(Page.GetByLabel("Confirm Password")).ToHaveValueAsync("Password!123");
    }
}