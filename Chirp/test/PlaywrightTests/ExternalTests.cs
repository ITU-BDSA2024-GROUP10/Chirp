using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests;

public class ExternalTests : PageTestWithDuende
{
    [Test]
    public async Task CreateUserUsingExternalProvider_AllInfo()
    {
        await Page.GotoAsync($"{RazorBaseUrl}/Identity/Account/Register");
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Page.WaitForURLAsync($"{DuendeBaseUrl}/Account/Login**");
        await Page.GetByPlaceholder("Username", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Username", new() { Exact = true }).FillAsync("TestWithAllInfo");
        await Page.GetByPlaceholder("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("Password", new() { Exact = true }).FillAsync("password");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
        
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with all info]");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [Mr. test with all" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with all info]");
    }
    
    [Test]
    public async Task CreateUserUsingExternalProvider_OnlyEmail()
    {
        await Page.GotoAsync($"{RazorBaseUrl}/Identity/Account/Register");
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("TestWithEmail");
        await Page.GetByPlaceholder("Password").ClickAsync();
        await Page.GetByPlaceholder("Password").FillAsync("password");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
        
        await Expect(Page.Locator("label")).ToContainTextAsync("Display Name");
        await Page.GetByPlaceholder("Please enter your display name").ClickAsync();
        await Page.GetByPlaceholder("Please enter your display name").FillAsync("Mr. test with email");
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with email]");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [Mr. test with email]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with email]");
    }
