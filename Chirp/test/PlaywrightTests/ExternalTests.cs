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
        
        await Expect(Page.Locator("label")).ToContainTextAsync("Username");
        await Page.GetByPlaceholder("Please enter your username").ClickAsync();
        await Page.GetByPlaceholder("Please enter your username").FillAsync("Mr. test with email");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with email]");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [Mr. test with email]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with email]");
    }
    
    [Test]
    public async Task CreateUserUsingExternalProvider_OnlyUsername()
    {
        await Page.GotoAsync($"{RazorBaseUrl}/Identity/Account/Register");
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("TestWithUserName");
        await Page.GetByPlaceholder("Password").ClickAsync();
        await Page.GetByPlaceholder("Password").FillAsync("password");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
        
        await Expect(Page.Locator("label")).ToContainTextAsync("Email");
        await Page.GetByPlaceholder("Please enter your email.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your email.").FillAsync("Test@withUsername.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [mr. Test with username]");

    }
    
    [Test]
    public async Task CreateUserUsingExternalProvider_NoInfo()
    {
        await Page.GotoAsync($"{RazorBaseUrl}/Identity/Account/Register");
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("TestWithNoInfo");
        await Page.GetByPlaceholder("Password").ClickAsync();
        await Page.GetByPlaceholder("Password").FillAsync("password");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
        
        await Expect(Page.Locator("form")).ToContainTextAsync("Email");
        await Expect(Page.Locator("form")).ToContainTextAsync("Username");
        await Page.GetByPlaceholder("Please enter your email.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your email.").FillAsync("Test@noInfo.com");
        await Page.GetByPlaceholder("Please enter your username").ClickAsync();
        await Page.GetByPlaceholder("Please enter your username").FillAsync("mr. test with no info");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [mr. test with no info]");
    }
}