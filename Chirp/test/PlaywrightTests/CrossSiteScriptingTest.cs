using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests;

[TestFixture]
public class CrossSiteScriptingTest : PageTestWithCustomWebApplicationFactory
{
    [Test]
    public async Task CrossSiteScripting_RegisterForm()
    {
        await Page.GotoAsync("/Identity/Account/Register");
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("<script>alert('alert!');</script>");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("mlao@itu.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("is invalid, can only contain letters or digits.");
    }
    
    [Test]
    public async Task CrossSiteScripting_loginForm()
    {
        await Page.GotoAsync("/Identity/Account/Login");
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("TestUSer");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("<script>alert('alert!');</script>");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("Invalid login attempt.");
    }
    
    [Test]
    public async Task CrossSiteScripting_CheepForm()
    {
        var user = new Author
        {
            UserName = "Mathias",
            Email = "test@itu.dk"
        };
        var password = "Password123!";
        
        await Page.GotoAsync("/Identity/Account/Register");
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(user.UserName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(user.Email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(user.UserName);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("<script>alert('alert!');</script>");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    }
}