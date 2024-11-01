using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class EndToEndTests : PageTestWithCustomWebApplicationFactory
{
    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync("/");
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
    }

    [Test]
    public async Task HasRegisterLink()
    {
        await Page.GotoAsync("/");

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();

        await Expect(Page).ToHaveTitleAsync(new Regex("Register"));
        // Expects page to have a heading with the name of Installation.
        //await Expect(Page.GetByRole(AriaRole.Form, new() { Name = "Installation" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task PasswordMustHaveNonAlphanumericCharacters()
    {
        //act
        await Page.GotoAsync("/Identity/Account/Register");
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("Mathias");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("mlao@itu.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Password123");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        //assert
        await Expect(Page.GetByText("Passwords must have at least")).ToBeVisibleAsync();
    }

    [Test]
    public async Task EndToEnd_RegisterLoginAndLogout()
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
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Mathias's Timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [Mathias]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        //assert
        await Expect(Page.GetByText("You have successfully logged")).ToBeVisibleAsync();
    }
}