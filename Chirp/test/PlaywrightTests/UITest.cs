using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace PlaywrightTests;

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
    [TestCase("?page=1", "-5")]
    [TestCase("?page=1", "0")]
    [TestCase("?page=2", "2")]
    [TestCase("?page=1", null)]
    public async Task PaginationOfPublicTimeline(string expectedEndpoint, string page)
    {
        //act
        await Page.GotoAsync($"/?page={page}");
        
        //assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}{expectedEndpoint}");
    }
    
    [Test]
    [TestCase("test?page=1", "-5")]
    [TestCase("test?page=1", "0")]
    [TestCase("test?page=2", "2")]
    [TestCase("test?page=1", null)]
    public async Task PaginationOfUserTimeline(string expectedEndpoint, string page)
    {
        //act
        await Page.GotoAsync($"/test?page={page}");
        
        //assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}{expectedEndpoint}");
    }
}