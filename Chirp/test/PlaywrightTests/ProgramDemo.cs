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
            }", 100); // Adjust speed (in milliseconds) between steps
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
            }", 100); // Adjust speed (in milliseconds) between steps
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
        
        var userToBeFolowowedByMRTest = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("UserToBeFolowowedByMRTest")
            .Create();

        var mrAuthor = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("Mr. Author")
            .Create();
        
        var userToFollowOtherUSer = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("UserToFollowUser")
            .GetTestAuthor();
        
        var randomUser = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("RandomUser")
            .Create();
        
        var userWhoWasFollowed = new TestAuthorBuilder(RazorFactory.GetUserManager())
            .WithUsernameAndEmail("UserWhoWasFollowed")
            .GetTestAuthor();
        
        
        userToFollowOtherUSer.AddFollow(userWhoWasFollowed);
        
        userToFollowOtherUSer.Create();
        
        var test = RazorFactory.GetDbContext().Authors.Select(a => new {a.UserName, a.Email, a.PasswordHash}).ToList();
        
        await GenerateCheeps(randomUser.Author, 300, "Random message");
        await GenerateCheep(userToBeFolowowedByMRTest.Author, $"Hello there people, im {userToBeFolowowedByMRTest.UserName}");
        await GenerateCheep(userToBeFolowowedByMRTest.Author, $"This is my second message");
        await GenerateCheep(mrAuthor.Author, "This is amazing!");
        
        #endregion
        
        //go to public timeline
        await Page.GotoAsync("/"); 
        await Page.WaitForURLAsync($"{RazorBaseUrl}/**");
        Thread.Sleep(5000);
        //scroll to bottom
        //await ScrollSmothlyToBottom();
        //scroll to top
        //await ScrollSmothlyToTop();
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
        Thread.Sleep(700);
        //Cheep From pirvate timeline
        await PostCheep("This is my first message!");
        Thread.Sleep(700);
        //Go to public timeline
        await RazorPageUtils.GoToPublicTimeline();
        //Cheep empty from public timeline
        await PostCheep("");
        Thread.Sleep(700);
        //Cheep from public timeline
        await PostCheep("This is my second message!");
        Thread.Sleep(700);
        //Go to private timeline
        await RazorPageUtils.GoToPrivateTimeline();
        Thread.Sleep(700);
        //Go to public timeline
        await RazorPageUtils.GoToPublicTimeline();
        //Follow user
        await FollowAuthor(userToBeFolowowedByMRTest);
        Thread.Sleep(700);
        await FollowAuthor(mrAuthor);
        Thread.Sleep(700);
        await FollowAuthor(randomUser);
        Thread.Sleep(700);
        //Unfollow user
        await UnfollowAuthor(randomUser);
        Thread.Sleep(700);
        //Go to private timeline
        await RazorPageUtils.GoToPrivateTimeline();
        //Unfollow user
        await UnfollowAuthor(userToBeFolowowedByMRTest);
        Thread.Sleep(700);
        //Refollow user
        await FollowAuthor(userToBeFolowowedByMRTest);
        Thread.Sleep(700);
        //Unfollow user
        await UnfollowAuthor(mrAuthor);
        Thread.Sleep(700);
        //Refresh page
        await Page.ReloadAsync();
        Thread.Sleep(700);
        //Go to about page
        await GoToAboutPage();
        Thread.Sleep(2000);
        //Forget user
        await ForgetAuthor();
        //Try to login
        await RazorPageUtils.Login(testAuthorToRegister);
        Thread.Sleep(300);
        //login third user, who was followed by external user
        await RazorPageUtils.Login(userToBeFolowowedByMRTest);
        //go to about me and see that there is now no followers
        await GoToAboutPage();
        Thread.Sleep(2000);
        //forget me
        await ForgetAuthor();
        //login 4th user
        await RazorPageUtils.Login(userWhoWasFollowed);
        //go to about me and see that there is now no one following
        await GoToAboutPage();
        Thread.Sleep(2000);
        //sleep for 10 seconds
        //Thread.Sleep(10000);
        Assert.True(true);
    }
    
    [Test]
    public async Task DemoOfExtraFeatures()
    {
        Assert.True(true);
    }
}