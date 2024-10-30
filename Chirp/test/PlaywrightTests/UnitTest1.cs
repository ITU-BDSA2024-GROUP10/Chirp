using System.Text.RegularExpressions;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

public class Tests : PageTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://bdsa2024group10chirpremotedb-h3c8bne5cahweegw.northeurope-01.azurewebsites.net/");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
    }
}