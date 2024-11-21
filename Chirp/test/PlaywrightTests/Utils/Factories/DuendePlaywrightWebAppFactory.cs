using Microsoft.Extensions.Hosting;
using TestUtilities;

namespace PlaywrightTests.Utils.Factories;

public class DuendePlaywrightWebAppFactory(string baseUrl) : DuendeWebAppFactory()
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host that is actually used by the
        // TestServer (In Memory).
        var testHost = base.CreateHost(builder);
        
        return CreateHostUsingKestrel.CreateHost(builder, testHost, baseUrl);
    }
}