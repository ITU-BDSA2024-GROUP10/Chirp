using System.Text.RegularExpressions;
using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;
using TestUtilities;

namespace PlaywrightTests;

//[Parallelizable(ParallelScope.Self)]
[TestFixture]
[NonParallelizable]
public class EndToEndTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    private async Task Register(Author author, string password)
    {
        await RazorPageUtils.Register(author.UserName!, author.Email!, password);
    }
    
    [Test]
    public async Task EndToEnd_RegisterLoginCheepAndLogout()
    {
        var author = TestUtils.CreateTestAuthor("Mr. test");
        var password = "Password123!";
        
        await Page.GotoAsync("/");
        await Register(author, password);
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("Cheep in my timeline");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Cheep in my timeline");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("Cheep in public timeline");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Cheep in public timeline");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = $"logout [{author.UserName!}]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        //assert
        await Expect(Page.GetByText("You have successfully logged")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChirpTextRedirectsToMainPage()
    {
        await Page.GotoAsync("http://localhost:5273/?page=1");
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Icon1 Chirp!" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        Assert.That(Page.Url, Is.EqualTo($"{RazorBaseUrl}/?page=1"));
    }

    [Test]
    public async Task ForgetMe()
    {
        #region Arrange

        var testAuthor1 = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("mr. test1")
            .GetTestAuthor();
            
        var testAuthor2 = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("mr. test2")
            .GetTestAuthor();
            
        var testAuthor3 = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("mr. test3")
            .GetTestAuthor();
        
        testAuthor1.AddFollow(testAuthor2);
        testAuthor3.AddFollow(testAuthor1);
        
        //because of the following relation, the other testauthors gets created when we create testAuthor1
        testAuthor1.Create(); 

        #endregion
        
        await RazorPageUtils.Login(testAuthor1);
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Yes, Delete" }).ClickAsync();
        
        var author1 = await RazorFactory.GetDbContext().Authors.FindAsync(testAuthor1.UserName);
        var author2Followers = RazorFactory.GetDbContext().Authors
            .Where(a => a.NormalizedUserName == testAuthor2.UserName!.ToUpper())
            .SelectMany(a => a.Followers);
        
        var author3Following = RazorFactory.GetDbContext().Authors
            .Where(a => a.NormalizedUserName == testAuthor3.UserName!.ToUpper())
            .SelectMany(a => a.Following);
        
        await Expect(Page.Locator("body")).ToContainTextAsync("login");
        Assert.That(Page.Url, Is.EqualTo($"{RazorBaseUrl}/?page=1"));
        Assert.That(author1, Is.Null);
        Assert.That(author2Followers, Is.Empty);
        Assert.That(author3Following, Is.Empty);
        
    }
}