using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private const string BaseUrl = "https://bdsa2024group10chirpremotedb-h3c8bne5cahweegw.northeurope-01.azurewebsites.net/";
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync(BaseUrl);

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
    }
    
    [Test]
    public async Task HasRegisterLink()
    {
        await Page.GotoAsync(BaseUrl);

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();

        await Expect(Page).ToHaveTitleAsync(new Regex("Register"));
        // Expects page to have a heading with the name of Installation.
        await Expect(Page.GetByRole(AriaRole.Form, new() { Name = "Installation" })).ToBeVisibleAsync();
    }
}