using Microsoft.Extensions.Hosting;
using TestUtils;

namespace PlaywrightTests.Utils;

//Adapted from microsofts test guide, for clarification:
//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
//Further Adapted from this Medium posts:
//https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-3-48c0edeff4b6
public class CustomWebApplicationFactory(string baseUrl) 
    : InMemoryCostumeWebApplicationFactory<Chirp.Web.Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host that is actually used by the
        // TestServer (In Memory).
        var testHost = base.CreateHost(builder);

        return CreateHostUsingKestrel.CreateHost(builder, testHost, baseUrl);
    }
}