using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace PlaywrightTests;

[NonParallelizable]
public class UITest : PageTestWithCustomWebApplicationFactory
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
    public async Task CheepBoxNotVisibleWhileLoggedOut()
    {
        await Page.GotoAsync("/");
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Log in to post Cheeps!" })).ToBeVisibleAsync();
    }
}