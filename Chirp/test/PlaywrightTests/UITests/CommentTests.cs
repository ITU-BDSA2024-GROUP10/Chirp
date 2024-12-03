using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

public class CommentTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    private TestAuthor _testAuthor;

    [SetUp]
    public async Task SetUp()
    {
        _testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("author")
            .Create();
        await GenerateCheep(_testAuthor.Author);
    }

    [Test]
    public async Task SpecificCheepViewExists()
    {
        await GenerateCheep(_testAuthor.Author, "this is author");
        await Page.GotoAsync("/");
        await Page.Locator("#messagelist div").Filter(new() { HasText = "—" }).GetByRole(AriaRole.Link).First
            .ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "author" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task CommentCountUpdates()
    {
        await GenerateCheep(_testAuthor.Author, "this is author");
        await RazorPageUtils.Login(_testAuthor);
        await Page.GotoAsync("/");
        await Expect(Page.Locator("li").Filter(new() { HasText = "author" }).Locator("small").Nth(1))
            .ToHaveTextAsync("0");
        await Page.Locator("#messagelist div").Filter(new() { HasText = "—" }).GetByRole(AriaRole.Link).First
            .ClickAsync();
        await Page.GetByPlaceholder("Write a comment").ClickAsync();
        await Page.GetByPlaceholder("Write a comment").FillAsync("This is a comment");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Post" }).ClickAsync();
        await Page.GetByText("This is a comment").ClickAsync();
        await Expect(Page.GetByText("1", new() { Exact = true })).ToHaveTextAsync("1");
    }

    [Test]
    public async Task TestAddComment()
    {
        await GenerateCheep(_testAuthor.Author, "this is author");
        await RazorPageUtils.Login(_testAuthor);
        await Page.GotoAsync("/");
        await Page.Locator("#messagelist div").Filter(new() { HasText = "—" }).GetByRole(AriaRole.Link).First
            .ClickAsync();
        await Page.GetByPlaceholder("Write a comment").ClickAsync();
        await Page.GetByPlaceholder("Write a comment").FillAsync("This is a comment");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Post" }).ClickAsync();
        await Expect(Page.GetByText("This is a comment")).ToBeVisibleAsync();
    }

    [Test]
    public async Task CannotPostCommentsWhenNotLoggedIn()
    {
        await GenerateCheep(_testAuthor.Author, "this is author");
        await Page.GotoAsync("/");
        await Page.Locator("#messagelist div").Filter(new() { HasText = "—" }).GetByRole(AriaRole.Link).First
            .ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Log in to comment on this" }))
            .ToBeVisibleAsync();
    }
}