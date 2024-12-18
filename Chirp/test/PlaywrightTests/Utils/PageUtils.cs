using Microsoft.Playwright;

namespace PlaywrightTests.Utils;

public class PageUtils(IPage page)
{   
    public async Task Login(TestAuthor testAuthor)
    {
        await Login(testAuthor.UserName!, testAuthor.Password);
    }
    
    public async Task Login(string userName, string password)
    {
        await page.GotoAsync("/");
        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
        await page.GetByPlaceholder("Username").ClickAsync();
        await page.GetByPlaceholder("Username").FillAsync(userName);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }

    public async Task Logout(TestAuthor testAuthor)
    {
        await Logout(testAuthor.UserName!);
    }
    
    public async Task Logout(string userName)
    {
        await page.GotoAsync("/");
        await page.GetByRole(AriaRole.Link, new() { Name = "logout ["+userName+"]" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }
    
    public async Task Register(TestAuthor testAuthor)
    {
        await Register(testAuthor.UserName!, testAuthor.Email!, testAuthor.Password!);
    }

    public async Task Register(string userName, string email, string password)
    {
        await page.GotoAsync("/Identity/Account/Register");
        await page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(userName);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(email);
        await page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await page.GetByLabel("Confirm Password").ClickAsync();
        await page.GetByLabel("Confirm Password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your account" }).ClickAsync();
    }
}