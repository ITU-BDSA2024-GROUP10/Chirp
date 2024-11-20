using System.Text.RegularExpressions;
using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests;

//[Parallelizable(ParallelScope.Self)]
[TestFixture]
[NonParallelizable]
public class FollowUnfollowTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [Test]
    public async Task UserCanFollowAuthor()
    {
       
    }

    [Test]
    public async Task UserCanUnfollowAuthor()
    {
       
    }

    [Test]
    public async Task UserCannotFollowSelf()
    {
       
    }

    [Test]
    public async Task CanOnlySeeOwnAndFollowedCheeps()
    {
       
    }

    [Test]
    public async Task UserStaysOnPageAfterUnfollowOnPrivateTimeline()
    {
       
    }

    [Test]
    public async Task UserStaysOnPageAfterFollowAndUnfollowOnPublicTimeline()
    {
       
    }
}