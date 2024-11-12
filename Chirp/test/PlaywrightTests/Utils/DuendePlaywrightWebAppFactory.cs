using System.Net.Sockets;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TestUtils.Duende;

namespace PlaywrightTests;

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