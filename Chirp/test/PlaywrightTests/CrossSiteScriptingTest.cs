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
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("mlao@itu.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.Locator("h3")).ToContainTextAsync("What's on your mind <script>alert('alert?");
    }
    
    [Test]
    public async Task CrossSiteScripting_loginForm()
    {
        await Page.GotoAsync("/Identity/Account/Login");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("mlao@itu.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("<script>alert('alert!');</script>");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToContainTextAsync("Invalid login attempt.");
    }
    
    [Test]
    public async Task CrossSiteScripting_CheepForm()
    {
        await Page.GotoAsync("/Identity/Account/Register");
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("Mathias");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@itu.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@itu.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("<script>alert('alert!');</script>");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    }
}