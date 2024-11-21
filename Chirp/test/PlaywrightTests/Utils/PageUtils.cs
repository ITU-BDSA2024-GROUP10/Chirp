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
}