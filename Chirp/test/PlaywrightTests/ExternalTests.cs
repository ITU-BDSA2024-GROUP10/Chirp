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
