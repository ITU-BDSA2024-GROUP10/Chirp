using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

public class PageButtonsTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [Test]
    public async Task FormatingTest_OnePage()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.Author, 32*1);
        
        await Page.GotoAsync("/?page=1");
        await Expect(Page.Locator("body")).Not.ToContainTextAsync("Next >");
    }
    
    [Test]
    public async Task FormatingTest_TwoPages()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.Author, 32*2);
        
        //first
        await Page.GotoAsync("/?page=1");
        await Expect(Page.Locator("body")).ToContainTextAsync("1 2 Next >");
        
        //second
        await Page.GotoAsync("/?page=2");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Prev 1 2");
    }
    
    [Test]
    public async Task FormatingTest_ThreePages()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.Author, 32*3);
        
        //first
        await Page.GotoAsync("/?page=1");
        await Expect(Page.Locator("body")).ToContainTextAsync("1 2 .. 3 Next >");
        
        //second
        await Page.GotoAsync("/?page=2");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Prev 1 2 3 Next >");
        
        //third
        await Page.GotoAsync("/?page=3");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Prev 1 .. 2 3");
    }
    
    [Test]
    public async Task FormatingTest_FivePages()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.Author, 32*5);
        
        //first
        await Page.GotoAsync("/?page=1");
        await Expect(Page.Locator("body")).ToContainTextAsync("1 2 .. 5 Next >");
        
        //second
        await Page.GotoAsync("/?page=2");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Prev 1 2 3 .. 5 Next >");
        
        //middle
        await Page.GotoAsync("/?page=3");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Prev 1 .. 2 3 4 .. 5 Next >");
        
        //second to last
        await Page.GotoAsync("/?page=4");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Prev 1 .. 3 4 5 Next >");
        
        //end
        await Page.GotoAsync("/?page=5");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Prev 1 .. 4 5");
    }
    
    [Test]
    public async Task PageNumbersAreClickable()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.Author, 32*5);
        
        //first
        await Page.GotoAsync("/?page=1");
        await Page.GetByRole(AriaRole.Link, new() { Name = "2" }).First.ClickAsync();
        Assert.That(Page.Url, Is.EqualTo($"{RazorBaseUrl}/?page=2"));
        await Page.GetByRole(AriaRole.Link, new() { Name = "5" }).First.ClickAsync();
        Assert.That(Page.Url, Is.EqualTo($"{RazorBaseUrl}/?page=5"));
    }
    
    [Test]
    public async Task CurrentPageIsBeyondLastPage()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.Author, 32*5);
        
        await Page.GotoAsync("/?page=6");
        await Expect(Page.Locator("body")).ToContainTextAsync("< Go to cheeps");
    }
}