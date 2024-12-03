using Chirp.Infrastructure.Model;
using CsvHelper;
using Microsoft.Playwright;
using PlaywrightTests.Utils;
using PlaywrightTests.Utils.PageTests;

namespace PlaywrightTests;

// dotnet test -s .\Utils\headed.runsettings --filter FullyQualifiedName~PlaywrightTests.ProgramDemo.DemoOfNormalFeatures

public class ProgramDemo : PageTestWithDuende
{
    
    private async Task ScrollSmothlyToBottom()
    {
        await Page.EvaluateAsync(@"(speed) => {
                const totalHeight = document.body.scrollHeight;
                const distance = 50; // Pixels to move each step
                let scrolled = 0;

                function step() {
                    scrolled += distance;
                    window.scrollBy(0, distance);

                    if (scrolled < totalHeight) {
                        setTimeout(() => window.requestAnimationFrame(step), speed);
                    }
                }

                step();
            }", 50); // Adjust speed (in milliseconds) between steps
    }
    
    private async Task ScrollSmothlyToTop()
    {
        await Page.EvaluateAsync(@"(speed) => {
                const distance = 50; // Pixels to move each step
                let scrolled = window.scrollY;

                function step() {
                    scrolled -= distance;
                    window.scrollBy(0, -distance);

                    if (scrolled > 0) {
                        setTimeout(() => window.requestAnimationFrame(step), speed);
                    } else {
                        window.scrollTo(0, 0); // Ensure we end at the very top
                    }
                }

                step();
            }", 50); // Adjust speed (in milliseconds) between steps
    }
    
    private async Task Register(TestAuthor testAuthor)
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(testAuthor.UserName);
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync(testAuthor.Email);
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(testAuthor.Password);
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync(testAuthor.Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync();
    }
    
    private async Task RegisterViaExternalProvider(TestAuthor testAuthor)
    {
        await Page.GotoAsync($"{RazorBaseUrl}/Identity/Account/Register");
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
        await Page.GetByPlaceholder("Username").ClickAsync();
        await Page.GetByPlaceholder("Username").FillAsync(testAuthor.UserName);
        await Page.GetByPlaceholder("Password").ClickAsync();
        await Page.GetByPlaceholder("Password").FillAsync(testAuthor.Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
    }
    
    private async Task LoginWithExternalProvider()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "OpenIdConnect" }).ClickAsync();
    }

    private async Task PostCheep(string message)
    {
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync(message);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    }
    
    private async Task FollowAuthor(TestAuthor testAuthor)
    {
        await Page.Locator("li").Filter(new() { HasText = testAuthor.UserName }).First.GetByRole(AriaRole.Button).ClickAsync();
    }
    
    private async Task UnfollowAuthor(TestAuthor testAuthor)
    {
        await Page.Locator("li").Filter(new() { HasText = testAuthor.UserName}).First.GetByRole(AriaRole.Button).ClickAsync();
    }
    
    private async Task GoToAboutPage()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
    }
    
    private async Task ForgetAuthor()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Yes, Delete" }).ClickAsync();
    }
    
    [Test]
    public async Task DemoOfNormalFeatures()
    {
        #region Setup

        var testAuthorToRegister = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("Mr. test")
            .GetTestAuthor();
        
        var externalAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsername("Mr. Demo")
            .WithEmail("MrDemo@Demo.com")
            .WithPassword("password")
            .GetTestAuthor();
        
        var userToBeFolowowedByExternalUser = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("UserToBeFolowowedByExternalUser")
            .GetTestAuthor();

        var mrAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("Mr. Author")
            .Create();
        
        var userToFollowOtherUSer = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("UserToFollowUser")
            .GetTestAuthor();
        
        var randomUser = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("RandomUser")
            .Create();
        
        
        userToFollowOtherUSer.AddFollow(userToBeFolowowedByExternalUser);
        
        userToFollowOtherUSer.Create();
        
        var test = RazorFactory.GetDbContext().Authors.Select(a => new {a.UserName, a.Email, a.PasswordHash}).ToList();
        
        await GenerateCheeps(randomUser.Author, 300, "Random message");
        await GenerateCheep(userToBeFolowowedByExternalUser.Author, $"Hello there people, im {userToBeFolowowedByExternalUser.UserName}");
        await GenerateCheep(userToBeFolowowedByExternalUser.Author, $"This is my second message");
        await GenerateCheep(mrAuthor.Author, "This is amazing!");
        
        #endregion
        
        //go to public timeline
        await Page.GotoAsync(RazorBaseUrl);
        //scroll to bottom
        await ScrollSmothlyToBottom();
        //scroll to top
        await ScrollSmothlyToTop();
        //Register user with external login
        await RegisterViaExternalProvider(externalAuthor);
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
        //Logout user
        await RazorPageUtils.Logout(externalAuthor.UserName);
        //Login user with external login
        await LoginWithExternalProvider();
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
        //Logout user
        await RazorPageUtils.Logout(externalAuthor.UserName);
        //Register user
        await Register(testAuthorToRegister);
        //Login user
        await RazorPageUtils.Login(testAuthorToRegister);
        //See private timeline
        await RazorPageUtils.GoToPrivateTimeline();
        //Cheep From pirvate timeline
        await PostCheep("This is my first message!");
        //Go to public timeline
        await RazorPageUtils.GoToPublicTimeline();
        //Cheep empty from public timeline
        await PostCheep("");
        //Cheep from public timeline
        await PostCheep("This is my second message!");
        //Go to private timeline
        await RazorPageUtils.GoToPrivateTimeline();
        //Go to public timeline
        await RazorPageUtils.GoToPublicTimeline();
        //Follow user
        await FollowAuthor(userToBeFolowowedByExternalUser);
        await FollowAuthor(mrAuthor);
        await FollowAuthor(randomUser);
        //Unfollow user
        await UnfollowAuthor(randomUser);
        //Go to private timeline
        await RazorPageUtils.GoToPrivateTimeline();
        //Unfollow user
        await UnfollowAuthor(userToBeFolowowedByExternalUser);
        //Refollow user
        await FollowAuthor(userToBeFolowowedByExternalUser);
        //Unfollow user
        await UnfollowAuthor(mrAuthor);
        //Refresh page
        await Page.ReloadAsync();
        //Go to about page
        await GoToAboutPage();
        //Forget user
        await ForgetAuthor();
        //Try to login
        await RazorPageUtils.Login(testAuthorToRegister);
        //login third user, who was followed by external user
        await RazorPageUtils.Login(userToBeFolowowedByExternalUser);
        //go to about me and see that there is now no followers
        await GoToAboutPage();
        //forget me
        await ForgetAuthor();
        //login 4th user
        await RazorPageUtils.Login(userToFollowOtherUSer);
        //go to about me and see that there is now no one following
        await GoToAboutPage();
        //sleep for 10 seconds
        // Thread.Sleep(10000);
        Assert.True(true);
    }
    
    [Test]
    public async Task DemoOfExtraFeatures()
    {
        
        
        Assert.True(true);
    }
}