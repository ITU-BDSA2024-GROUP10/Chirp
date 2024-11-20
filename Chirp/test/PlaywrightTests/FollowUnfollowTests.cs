using Chirp.Infrastructure.Model;
using Microsoft.Playwright;
using PlaywrightTests.Utils;


namespace PlaywrightTests;

//[Parallelizable(ParallelScope.Self)]
[TestFixture]
[NonParallelizable]
public class FollowUnfollowTests : PageTestWithRazorPlaywrightWebApplicationFactory
{
    [SetUp]
    public async Task SetUp()
    {
        // register and login mr. follower
        var author = new Author
        {
            UserName = "author",
            Email = "author@itu.dk"
        };
        var password = "Password123!";

        // register author
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(author.UserName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(author.Email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync(); 

        // login author
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(author.UserName);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // write test cheep
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("test");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        // logout
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [author]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        // register and login mr. follower
        var follower = new Author
        {
            UserName = "follower",
            Email = "follower@itu.dk"
        };

        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(follower.UserName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(follower.Email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync(); 

        // login
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(follower.UserName);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }
    
    [Test]
    public async Task UserCanFollowAuthor()
    {
        await Page.GetByText("author follow test").ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();

        await Expect(Page.GetByText("author unfollow test")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCanUnfollowAuthor()
    {
        await Page.GetByText("author follow test").ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.GetByText("author unfollow test").ClickAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "unfollow" }).ClickAsync();
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserCannotFollowSelf()
    {   
        // logout from follower
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [follower]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        // login to author
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("author");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // assert that there is no follow button for the author's own cheep
        var followButton = Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button);

        // expect the follow button to be zero
        await Expect(followButton).ToBeHiddenAsync();
    }

    [Test]
    public async Task CanOnlySeeOwnAndFollowedCheeps()
    {
        // logout of follower
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [follower]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        // register and login author2
        var author2 = new Author
        {
            UserName = "author2",
            Email = "author2@itu.dk"
        };
        var password = "Password123!";
        
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(author2.UserName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(author2.Email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync();

        // login as author2
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(author2.UserName);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // write a cheep as author2
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("this is author2's cheep");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        // logout of author2
        await Page.GetByRole(AriaRole.Link, new() { Name = "logout [author2]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        // login back as follower
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync("follower");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // follow author1
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button).ClickAsync();

        // go to private timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();

        // assert that author2's cheep is not on private timeline
        await Expect(Page.Locator("li").Filter(new() { HasText = "this is author2's cheep" })).ToBeHiddenAsync();

        // assert that author1's cheep is visible
        await Expect(Page.Locator("li").Filter(new() { HasText = "author unfollow test" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserStaysOnPageAfterUnfollowOnPrivateTimeline()
    {
        // follow author
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button).ClickAsync();

        // go to follower's timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();

        // unfollow author
        await Page.Locator("li").Filter(new() { HasText = "author unfollow test" }).GetByRole(AriaRole.Button).ClickAsync();

        // assert that page still shows no cheeps after unfollowing, indicating user is still on the private timeline
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task UserStaysOnPageAfterFollowAndUnfollowOnPublicTimeline()
    {
       // follow author
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "author follow test" }).GetByRole(AriaRole.Button).ClickAsync();

        // unfollow author
        await Page.Locator("li").Filter(new() { HasText = "author unfollow test" }).GetByRole(AriaRole.Button).ClickAsync();

        // assert that the user is still on the public timeline by checking the cheep
        await Expect(Page.Locator("li").Filter(new() { HasText = "author follow test" })).ToBeVisibleAsync();

        // assert that "there are no cheeps so far" message does not exist
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeHiddenAsync();
    }
}