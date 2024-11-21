using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests;

public class ExternalTests : PageTestWithDuende
{
    private async Task registerViaExternalProvider(string username, string password)
    {
        await Page.GotoAsync($"{RazorBaseUrl}/Identity/Account/Register");
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(username);
        await Page.GetByPlaceholder("Password").ClickAsync();
        await Page.GetByPlaceholder("Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
    }

    private async Task logout()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout " }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }

    private async Task loginWithExternalProvider()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
    }
    
    [Test]
    public async Task CreateUserUsingExternalProvider_AllInfo()
    {
        await registerViaExternalProvider("TestWithAllInfo", "password");
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with all info]");

        await logout();
        
        await loginWithExternalProvider();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with all info]");
        
    }
    
    [Test]
    public async Task CreateUserUsingExternalProvider_OnlyEmail()
    {
        await registerViaExternalProvider("TestWithEmail", "password");
        
        await Expect(Page.Locator("label")).ToContainTextAsync("Username");
        await Page.GetByPlaceholder("Please enter your username").ClickAsync();
        await Page.GetByPlaceholder("Please enter your username").FillAsync("Mr. test with email");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with email]");

        await logout();
        await loginWithExternalProvider();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [Mr. test with email]");
    }
    
    [Test]
    public async Task CreateUserUsingExternalProvider_OnlyUsername()
    {
        await registerViaExternalProvider("TestWithUserName", "password");
        
        await Expect(Page.Locator("label")).ToContainTextAsync("Email");
        await Page.GetByPlaceholder("Please enter your email.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your email.").FillAsync("Test@withUsername.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();

        await loginWithExternalProvider();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [mr. Test with username]");

    }
    
    [Test]
    public async Task CreateUserUsingExternalProvider_NoInfo()
    {
        await registerViaExternalProvider("TestWithNoInfo", "password");
        
        await Expect(Page.Locator("form")).ToContainTextAsync("Email");
        await Expect(Page.Locator("form")).ToContainTextAsync("Username");
        await Page.GetByPlaceholder("Please enter your email.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your email.").FillAsync("Test@noInfo.com");
        await Page.GetByPlaceholder("Please enter your username").ClickAsync();
        await Page.GetByPlaceholder("Please enter your username").FillAsync("mr. test with no info");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();

        await loginWithExternalProvider();
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [mr. test with no info]");
    }
}