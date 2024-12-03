using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

[NonParallelizable]
public class USerTimelineTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [Test]
    [TestCase("?page=1", "-5")]
    [TestCase("?page=1", "0")]
    [TestCase("?page=2", "2")]
    [TestCase("?page=1", null)]
    public async Task PaginationOfUserTimeline(string expectedEndpoint, string page)
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("Mr.test")
            .Create();
        
        //act
        await Page.GotoAsync($"/{testAuthor.UserName!.ToLower()}?page={page}");

        //assert
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}/{testAuthor.UserName.ToLower()}{expectedEndpoint}");
    }
    
    [Test]
    public async Task SeeCorrectNumberOfCheepsOnUserTimeline()
    {
        #region Arrange
        var realTestAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("Mr. real")
            .Create();
        var fakeTestAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("Mr. fake")
            .Create();
        await GenerateCheeps(realTestAuthor.Author, 33);
        await GenerateCheeps(fakeTestAuthor.Author, 33);

        #endregion

        //act
        await Page.GotoAsync($"/{realTestAuthor.UserName}");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(32);
        await Page.GotoAsync($"/{realTestAuthor.UserName}?page=2");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(1);
        await Page.GotoAsync($"/{realTestAuthor.UserName}?page=3");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(0);
    }
    
    [Test]
    public async Task TheUserTimeLineIsCaseInSensitive()
    {
        #region Arrange
        
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .WithUsername("MR. tESt")
            .Create();
        var cheep = await GenerateCheep(testAuthor.Author);
        
        #endregion

        //act
        await Page.GotoAsync($"/{testAuthor.UserName!.ToUpper()}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
        
        await Page.GotoAsync($"/{testAuthor.UserName.ToLower()}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
        
        await Page.GotoAsync($"/{testAuthor.UserName}");
        await Expect(Page.Locator("#messagelist"))
            .ToContainTextAsync(cheep.Message);
    }
}