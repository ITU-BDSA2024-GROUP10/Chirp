﻿using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

[NonParallelizable]
public class CheepBoxTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [Test]
    public async Task CheepBoxNotVisibleWhileLoggedOut()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        
        await Page.GotoAsync("/");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Log in to post Cheeps!" }))
            .ToBeVisibleAsync();
        
        await Page.GotoAsync($"/{testAuthor.UserName}");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Log in to post Cheeps!" }))
            .Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task CheepBoxVisibleWhileLoggedIn()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        
        await RazorPageUtils.Login(testAuthor);
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"What's on your mind {testAuthor.UserName}?" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#Message")).ToBeVisibleAsync();

        await Page.GotoAsync($"/{testAuthor.UserName}");
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"What's on your mind {testAuthor.UserName}?" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#Message")).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task CheepBoxNotVisibelOnOtherAuthorsTimeline()
    {
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        var otherAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("Mr. other")
            .Create();
        
        await RazorPageUtils.Login(testAuthor);
        await Page.GotoAsync($"/{otherAuthor.UserName}");
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"What's on your mind {testAuthor.UserName}?" }))
            .Not.ToBeVisibleAsync();
        await Expect(Page.Locator("#Message")).Not.ToBeVisibleAsync();
    }
}