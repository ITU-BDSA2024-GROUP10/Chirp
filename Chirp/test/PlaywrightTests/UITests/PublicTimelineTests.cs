using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

[NonParallelizable]
public class PublicTimelineTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
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
        await Expect(Page).ToHaveURLAsync($"{RazorBaseUrl}/{expectedEndpoint}");
    }
    
    [Test]
    public async Task SeeCorrectNumberOfCheepsOnPublicTimeline()
    {
        //arrange
        var testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithDefault()
            .Create();
        await GenerateCheeps(testAuthor.Author, 33);

        //act
        await Page.GotoAsync("/");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(32);
        await Page.GotoAsync("/?page=2");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(1);
        await Page.GotoAsync("/?page=3");
        await Expect(Page.Locator("#messagelist > li")).ToHaveCountAsync(0);
    }
}