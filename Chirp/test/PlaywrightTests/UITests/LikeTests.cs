using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests.UITests;

public class LikeTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    private TestAuthor _testAuthor;

    [SetUp]
    public async Task SetUp()
    {
        _testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("author")
            .Create();
        await GenerateCheep(_testAuthor.author);

        await GenerateCheep(_testAuthor.author, "this is author");
        await RazorPageUtils.Login(_testAuthor);
        await Page.GotoAsync("/");
    }
}