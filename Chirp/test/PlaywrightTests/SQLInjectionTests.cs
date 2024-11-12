using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests;

public class SQLInjectionTests : PageTestWithCustomWebApplicationFactory
{
    [Test]
    public async Task SQLInjectionInCheepTextBox()
    {
        //act
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
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("mlao@itu.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("105; DROP TABLE Cheeps;");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        
        //assert
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("105; DROP TABLE Cheeps;");
    }

    [Test]
    public async Task SQLInjectionInRegisterNameAndLoggingIn()
    {
        //act
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("DROP TABLE Cheeps;");
        await Page.Locator("#registerForm div").Filter(new() { HasText = "Name" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("cheep");
        await Page.GetByPlaceholder("name@example.com").FillAsync("cheep@gmail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password!123");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password!123");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("cheep");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Alt+@");
        await Page.GetByPlaceholder("name@example.com").FillAsync("cheep@gmail.com");
        await Page.Locator("#account div").Nth(1).ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Password!123");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        //assert
        await Expect(Page.Locator("body")).ToContainTextAsync("logout [DROP TABLE Cheeps;]");
    }
}