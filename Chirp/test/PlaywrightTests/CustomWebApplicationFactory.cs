using System.Data.Common;
using System.Net.Sockets;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestUtils;

namespace PlaywrightTests;

//Adapted from microsofts test guide, for clarification:
//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
//Further Adapted from this Medium posts:
//https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-3-48c0edeff4b6
public class CustomWebApplicationFactory(String baseUrl) : InMemoryCostumeWebApplicationFactory
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host that is actually used by the
        // TestServer (In Memory).
        var testHost = base.CreateHost(builder);

        // configure and start the actual host using Kestrel.
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());

        var host = builder.Build();
        host.Start();
        
        // Wait until the server is listening
        WaitUntilServerIsAvailable(baseUrl);
        
        // In order to cleanup and properly dispose HTTP server
        // resources we return a composite host object that is
        // actually just a way to intercept the StopAsync and Dispose
        // call and relay to our HTTP host.
        return new CompositeHost(testHost, host);
    }
    
    private void WaitUntilServerIsAvailable(string url)
    {
        var uri = new Uri(url);
        using var client = new TcpClient();

        var maxAttempts = 5;
        var attempt = 0;

        while (attempt < maxAttempts)
        {
            try
            {
                client.Connect(uri.Host, uri.Port);
                if (client.Connected)
                {
                    break;
                }
            }
            catch
            {
                attempt++;
                Thread.Sleep(1000);
            }
        }

        if (attempt == maxAttempts)
        {
            throw new Exception("Server did not start in time.");
        }
    }
}