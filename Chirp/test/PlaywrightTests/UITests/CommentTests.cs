using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Playwright;
using NUnit.Framework.Internal;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;
namespace PlaywrightTests.UITests;

public class CommentTests :  PageTestWithRazorPlaywrightWebApplicationFactory
{
    private TestAuthor _testAuthor;
    
    [SetUp]
    public async Task SetUp()
    {
        _testAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("author")
            .Create();
        await GenerateCheep(_testAuthor.author);

    }
}